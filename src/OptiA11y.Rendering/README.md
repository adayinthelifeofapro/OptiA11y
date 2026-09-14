# OptiA11y.Rendering

Optional slice that captures **actual rendered/computed behaviour** of a page using headless
Chromium via [Playwright](https://playwright.dev/dotnet/): computed CSS for text (colors,
background, font size, font weight, text-align), the rendered bounding box and focus behaviour of
every interactive element, elements with automatic infinite animation, whether the page reflows
at a narrow viewport, and whether text clips under the WCAG 1.4.12 reference spacing overrides.
This complements — it does not replace — `OptiA11y.Core`'s default `HtmlFragmentParser`, which can
only see inline `style=""` attributes and static markup, and therefore misses anything driven by
external stylesheets, CSS classes, theme styling, or actual browser layout/interaction.

## Why a separate slice

`OptiA11y.Core` is intentionally CMS-agnostic and dependency-light (only HtmlAgilityPack). Adding
a full browser engine there would break that constraint for every host, even ones that never want
rendered-style checks. Keeping rendering in its own project means:

- `OptiA11y.Core` and `OptiA11y.Cms`'s default `AddOptiA11y()` registration are completely
  unaffected — no new dependency, no behavior change.
- Hosts that want real computed-style/layout checks opt in explicitly via
  `services.AddOptiA11yRenderedStyles()`.
- The rendered-vs-inline distinction is transparent: the rules that consume rendered-only
  fragments (`ColorContrastRule`, `TextReadabilityRule`, `TargetSizeRule`, `FocusIndicatorRule`,
  `MotionRule`, `ReflowRule`, `TextSpacingRule`, all in Core) don't know or care where their
  fragments came from — they just evaluate whatever fragment instances exist on the
  `AuditDocument`.

## How it fits together

1. `IRenderedStyleProvider.CaptureAsync(Uri)` renders a page and returns a `RenderedPageDiagnostics`:
   a `RenderedTextStyle` per visible text node (computed `color`, effective `background-color`
   plus whether that background is an image/gradient, `font-size`, `font-weight`, `text-align`), a
   `RenderedElementDiagnostics` per interactive element (rendered bounding box, whether focusing it
   changes its appearance at all), short descriptions of elements with an automatic infinite
   animation, whether the page overflows at a 320px-equivalent viewport, and text samples that
   clip under the WCAG 1.4.12 reference spacing overrides.
2. `PlaywrightRenderedStyleProvider` is the only implementation, using headless Chromium. It fails
   soft (returns `RenderedPageDiagnostics.Empty`) if the browser can't be launched or the page
   can't be reached, so a missing/misconfigured browser never breaks an audit. Internally it makes
   three passes over one page load: the default viewport (text styles, element diagnostics,
   animation scan), a resized 320px-wide viewport (reflow), and the default viewport again with
   the WCAG 1.4.12 stylesheet injected (text spacing).
3. `RenderedStyleFragmentBuilder` converts a `RenderedPageDiagnostics` into the same
   `ColorContrastFragment`/`TextStyleFragment`/`TargetSizeFragment`/`FocusIndicatorFragment`/
   `MotionFragment`/`ReflowFragment`/`TextSpacingFragment` shapes Core's rules expect, reusing
   Core's `ColorContrastCalculator` for the WCAG contrast-ratio math.
4. `RunAuditHandler` (in `OptiA11y.Cms`) appends these fragments to the `AuditDocument` *before*
   running the rule engine, but only when both an `IRenderedStyleProvider` and a resolvable
   `IContentPreviewUrlResolver` URL are registered/available — otherwise this step is a no-op.

## Enabling this slice in a host

```csharp
services.AddOptiA11y();                 // existing rule set, unaffected
services.AddOptiA11yRenderedStyles();   // opt-in: registers PlaywrightRenderedStyleProvider

// Required: hosts must supply their own resolver so a preview URL can be built per content item.
// The default NullContentPreviewUrlResolver returns null, which skips rendered-style enrichment entirely.
services.AddSingleton<IContentPreviewUrlResolver, MyContentPreviewUrlResolver>();
```

### Prerequisite: install Playwright's browser binary

Playwright's browser binaries are not included in the NuGet package and must be installed once
per machine/build agent:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

If this step is skipped, `PlaywrightRenderedStyleProvider.CaptureAsync` fails soft and returns
`RenderedPageDiagnostics.Empty` — audits continue to work using inline-style-only fragments from
`HtmlFragmentParser`, and the rendered-only rules simply find nothing to evaluate.

## Known limitations

- Correlating rendered DOM text nodes back to a specific `SourceLocation` is best-effort: fragments
  are matched by trimmed visible text content only, since Core's fragment model has no DOM/element
  reference. Pages with duplicate identical text may produce ambiguous correlation.
- Requires a resolvable public/preview URL per content item. Building that URL is CMS/site-specific
  and is intentionally left to the host via `IContentPreviewUrlResolver` — no default implementation
  is provided beyond the no-op fallback.
- Adds meaningful latency (a full browser page load, plus two extra passes over that page, per
  audited item), so this should typically be run on-demand rather than as part of every save,
  unlike the fast inline-style checks.
- `target-size` excludes inline text links and native checkbox/radio inputs (the exceptions WCAG
  2.5.8 itself carves out), but cannot detect the SC's other exceptions (essential size, adjacent
  spacing) — a flagged element may still turn out to be a legitimate exception on manual review.
- `motion` and `reflow` cannot verify the "essential" and "2D-layout-required" exceptions their
  success criteria allow, which is why both report `NeedsReview` rather than `Fail`.
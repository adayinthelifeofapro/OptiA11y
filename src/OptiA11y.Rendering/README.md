# OptiA11y.Rendering

Optional slice that captures **actual rendered/computed CSS** (colors, background, font size,
font weight, text-align) for text on a page, using headless Chromium via
[Playwright](https://playwright.dev/dotnet/). This complements — it does not replace —
`OptiA11y.Core`'s default `HtmlFragmentParser`, which can only see inline `style=""` attributes
and therefore misses anything driven by external stylesheets, CSS classes, or theme styling.

## Why a separate slice

`OptiA11y.Core` is intentionally CMS-agnostic and dependency-light (only HtmlAgilityPack). Adding
a full browser engine there would break that constraint for every host, even ones that never want
rendered-style checks. Keeping rendering in its own project means:

- `OptiA11y.Core` and `OptiA11y.Cms`'s default `AddOptiA11y()` registration are completely
  unaffected — no new dependency, no behavior change.
- Hosts that want real computed-style checks opt in explicitly via
  `services.AddOptiA11yRenderedStyles()`.
- The rendered-vs-inline distinction is transparent: `ColorContrastRule` and
  `TextReadabilityRule` (in Core) don't know or care where their fragments came from — they just
  evaluate whatever `ColorContrastFragment`/`TextStyleFragment` instances exist on the
  `AuditDocument`.

## How it fits together

1. `IRenderedStyleProvider.CaptureAsync(Uri)` renders a page and returns a `RenderedTextStyle` per
   visible text node (computed `color`, effective `background-color`, `font-size`, `font-weight`,
   `text-align`).
2. `PlaywrightRenderedStyleProvider` is the only implementation, using headless Chromium. It fails
   soft (returns an empty list) if the browser can't be launched or the page can't be reached, so
   a missing/misconfigured browser never breaks an audit.
3. `RenderedStyleFragmentBuilder` converts those into the same `ColorContrastFragment`/
   `TextStyleFragment` shapes `HtmlFragmentParser` produces from inline styles, reusing Core's
   `ColorContrastCalculator` for the WCAG contrast-ratio math.
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

If this step is skipped, `PlaywrightRenderedStyleProvider.CaptureAsync` fails soft and returns an
empty list — audits continue to work using inline-style-only fragments from `HtmlFragmentParser`.

## Known limitations

- Correlating rendered DOM text nodes back to a specific `SourceLocation` is best-effort: fragments
  are matched by trimmed visible text content only, since Core's fragment model has no DOM/element
  reference. Pages with duplicate identical text may produce ambiguous correlation.
- Requires a resolvable public/preview URL per content item. Building that URL is CMS/site-specific
  and is intentionally left to the host via `IContentPreviewUrlResolver` — no default implementation
  is provided beyond the no-op fallback.
- Adds meaningful latency (a full browser page load per audited item), so this should typically be
  run on-demand rather than as part of every save, unlike the fast inline-style checks.

# OptiA11y

OptiA11y is editorial accessibility assistance for Optimizely content authors. It is **not a
compliance certification tool**. A clean audit result means OptiA11y found no issues it was able
to detect — it is not a legal position, and it should never be presented to a client, auditor, or
regulator as proof of WCAG or ADA conformance. Treat OptiA11y as a second pair of eyes for
editors, not as a substitute for a professional accessibility audit.

## What it does

OptiA11y evaluates content **at the property level** — alt text quality, link purpose and naming,
heading structure, table semantics, form labels and input purpose, button names, iframe titles,
deprecated markup, interactive-attribute misuse (including invalid ARIA and nested interactive
controls), fake/manual list and heading structures, language declarations (including inline
passages in another script), media captions/transcripts/audio description/autoplay, page titling,
colour contrast, reading level, sensory-characteristic instructions, and text readability — without
rendering a page or running a browser by default. This is the differentiated editorial value: it is
the analysis Siteimprove and similar crawler-based tools handle least well, and it works
identically whether content lives on PaaS (via `IContentLoader`) or SaaS (via the Content Delivery
API).

An optional, separately packaged slice (`OptiA11y.Rendering`) adds real headless-browser rendering
so contrast and readability rules can evaluate actual computed CSS instead of only inline styles —
see [Rendered-style enrichment (optional)](#rendered-style-enrichment-optional) below.

## The Confidence model — the honesty mechanism

Every finding carries a `Confidence`:

- **`Fail`** — a deterministic, unambiguous violation (a missing alt attribute, a skipped heading
  level, an empty heading, a missing form label, a contrast ratio below threshold). Reserved for
  structural facts a rule can be certain about.
- **`NeedsReview`** — a heuristic judgement (alt text quality, link text descriptiveness, whether
  manually bulleted text is really a list). The tool cannot know editorial intent, so it flags
  something worth a human's attention rather than asserting a defect.

Heuristic rules must never report `Fail`. This distinction is what stops OptiA11y from
manufacturing false assurance, and it is enforced by a dedicated test
(`HeuristicRulesNeverFailTests`) in `tests/OptiA11y.Core.Tests`.

## Rule catalogue

All rules live in `OptiA11y.Core.Rules` and implement `IContentRule`. Each is independently
registered in DI (`ServiceCollectionExtensions.AddOptiA11y`), so hosts can add or remove rules
without touching the engine.

| Rule ID | WCAG SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `alt-text-quality` | 1.1.1 | A | Fail + NeedsReview | Missing `alt` is `Fail`; low-quality/placeholder alt text is `NeedsReview` |
| `svg-accessible-name` | 1.1.1 | A | Fail | Inline SVG, image-map `<area>`, or `<object>`/`<embed>` with no accessible name and not `aria-hidden` |
| `heading-structure` | 1.3.1 | A | Fail + NeedsReview | Skipped heading levels, empty headings, and multiple H1s are `Fail`; a missing H1 (may come from the template) and headings that read as body copy are `NeedsReview` |
| `faux-heading` | 1.3.1 | A | NeedsReview | A short, fully-bold or enlarged paragraph outside any real heading element |
| `link-purpose` | 2.4.4 | A | NeedsReview | Non-descriptive link text (e.g. "click here") |
| `link-name` | 4.1.2 | A | Fail | Links with no accessible name at all (no text, aria-label, or alt on an inner image) |
| `document-link-expectations` | 2.4.4 | A | NeedsReview | A link to a downloadable document (PDF, DOCX, etc.) whose text doesn't say so |
| `table-headers` | 1.3.1 | A | Fail + NeedsReview | Missing header cells is `Fail`; missing caption is `NeedsReview` |
| `table-complexity` | 1.3.1 | A | Fail | Merged cells (rowspan/colspan) without header `scope`, or rows with inconsistent cell counts |
| `media-captions` | 1.2.2 | A | Fail + NeedsReview | Video without captions/transcript is `Fail`; audio-only is `NeedsReview` |
| `audio-description` | 1.2.5 | AA | NeedsReview | Video with no audio-description track |
| `autoplay-media` | 1.4.2 | A | Fail | Audio/video that autoplays unmuted with no `controls` |
| `form-labels` | 1.3.1 | A | Fail | Form controls with no accessible name (label/aria-label/aria-labelledby) |
| `fieldset-legend` | 1.3.1 | A | Fail | `<fieldset>` with no `<legend>`, or a same-`name` radio/checkbox group with no wrapping fieldset |
| `input-purpose` | 1.3.5 | AA | NeedsReview | A field that looks like it collects a common identity value (email, phone, etc.) with no `autocomplete` |
| `button-name` | 4.1.2 | A | Fail | Buttons/button-like inputs with no accessible name |
| `title-attribute-misuse` | 4.1.2 | A | NeedsReview | A link/button whose `title` duplicates its visible text, or is its only accessible name |
| `iframe-title` | 4.1.2 | A | Fail | Iframes with no `title`/aria-label |
| `deprecated-elements` | 1.3.1 | A | Fail | Obsolete presentational tags (`<blink>`, `<marquee>`, `<font>`, `<center>`, etc.) |
| `interactive-attributes` | 2.4.3 | A | Fail | `aria-hidden` on natively interactive elements, positive `tabindex`, duplicate `accesskey` |
| `nested-interactive` | 4.1.2 | A | Fail | A natively interactive element (link/button/form control) nested inside another one |
| `invalid-aria` | 4.1.2 | A | Fail | A `role` or `aria-*` attribute name that isn't part of the ARIA specification |
| `broken-aria-reference` | 4.1.2 | A | NeedsReview | An `aria-labelledby`/`aria-describedby`/`for` reference that doesn't resolve within the same property |
| `list-structure` | 1.3.1 | A | NeedsReview | Manually bulleted/numbered text that should be a real `<ul>`/`<ol>` |
| `language-attribute` | 3.1.1 | A | Fail | Missing or malformed `lang` attribute |
| `language-of-parts` | 3.1.2 | AA | NeedsReview | A passage in a different writing system with no `lang` override marking it |
| `sensory-characteristics` | 1.3.3 | A | NeedsReview | Instructions that rely on shape, position, or color alone (e.g. "the button on the right") |
| `page-title` | 2.4.2 | A | Fail + NeedsReview | A completely empty content name is `Fail`; a generic placeholder name is `NeedsReview` |
| `color-contrast` | 1.4.3 | AA | Fail + NeedsReview | Insufficient contrast ratio between inline (or rendered) foreground/background colors is `Fail`; text over a rendered image/gradient background is `NeedsReview` (the sampled color there is unreliable) |
| `text-readability` | 1.4.8 | AAA | Fail + NeedsReview | Fully justified text or font sizes under 12px are `Fail`; long runs of ALL CAPS text are `NeedsReview` |
| `reading-level` | 3.1.5 | AAA | NeedsReview | A passage that scores as very difficult on the Flesch Reading Ease scale |
| `target-size` *(rendered-style only)* | 2.5.8 | AA | Fail | An interactive element rendering smaller than 24x24px (excludes inline text links and native checkbox/radio, per the SC's own exceptions) |
| `focus-indicator` *(rendered-style only)* | 2.4.7 | AA | Fail | A focusable element with no visible change (outline/box-shadow/background/border) when it receives keyboard focus |
| `motion` *(rendered-style only)* | 2.2.2 | A | NeedsReview | An element with a CSS animation that repeats indefinitely and starts automatically |
| `reflow` *(rendered-style only)* | 1.4.10 | AA | NeedsReview | The page requires horizontal scrolling at a 320px-equivalent viewport |
| `text-spacing` *(rendered-style only)* | 1.4.12 | AA | Fail | Text that visually clips once the WCAG reference text-spacing overrides are applied |

Rows marked *(rendered-style only)* only ever produce findings when the optional
[`OptiA11y.Rendering`](#rendered-style-enrichment-optional) slice is registered — otherwise their
underlying fragments are never produced, and the rule is simply a no-op.

## Architecture

Ports and adapters at the core, vertical slices at the CMS layer:

```
src/
  OptiA11y.Core/       Rules engine, model, and content rules. No Optimizely dependency.
  OptiA11y.Cms/         Razor Class Library: PaaS adapter, RunAudit feature, DI wiring, editor UI.
  OptiA11y.Saas/        REST API adapter, isolated SaaS response mapping.
  OptiA11y.Rendering/   Optional slice: headless-browser computed-style capture (see below).
tests/
  OptiA11y.Core.Tests/        Rule fixtures, no CMS dependency.
  OptiA11y.Adapters.Tests/    PaaS/SaaS adapter equivalence and endpoint tests.
  OptiA11y.Rendering.Tests/   Rendered-style provider and fragment-builder tests.
sample/
  OptiA11y.SampleSite/        Seeded content with deliberate accessibility problems.
```

`OptiA11y.Core` has zero Optimizely dependencies and is fully unit-testable without a CMS
present. Rules operate against a normalised `AuditDocument` made of typed fragments (image, link,
heading, table, text, media, form field, button, iframe, deprecated element, interactive
attributes, list structure, language attribute, color contrast, text style, non-text element
(SVG/area/object/embed), ARIA attributes, ARIA reference, fieldset, radio group, emphasis block,
page metadata, and the rendered-style-only target size/focus indicator/motion/reflow/text spacing
fragments), each carrying a `SourceLocation` (content reference, property name, nested block
path, ordinal) that identifies exactly where the finding came from — this is the foundation for
deep-linking a finding back to the offending property, including inside nested blocks in a
content area.

`HtmlFragmentParser` (in `OptiA11y.Core.Parsing`) is the single, shared entry point that turns raw
HTML into fragments for every adapter — PaaS, SaaS, and (via enrichment) rendered-style capture all
funnel through the same parsing logic, so rule behaviour is identical regardless of content source.

## CMS 13 add-on

`OptiA11y.Cms` is a true installable add-on for Optimizely CMS 13 - no code needs to be added to
the host solution. Install the package and it self-registers via:

- `Infrastructure/OptiA11yCmsModule.cs` - an `IConfigurableModule` that registers all OptiA11y
  services (rule set, rule engine, RunAudit handler, and a real `IContentLoader`-backed
  `EPiServerPaasContentLoader`) into the host's DI container automatically during EPiServer's
  initialization pipeline.
- `Infrastructure/Navigation/OptiA11yAuditPanelPlugin.cs` - registers an in-context editor panel
  (an iframe-based view) so editors can view accessibility findings for the page they are
  currently editing, without leaving the edit view.
- `Infrastructure/OptiA11yStartupFilter.cs` - an `IStartupFilter`, registered by
  `OptiA11yCmsModule`, that maps the RunAudit endpoint (and the editor panel endpoint) into the
  host's ASP.NET Core pipeline automatically. Hosts do not need to call `MapRunAuditEndpoint()`
  themselves.

`EPiServerPaasContentLoader` maps `IContent`/`XhtmlString`/`ContentArea` (including nested blocks)
into the CMS-agnostic `PaasContentNode`/`PaasProperty` shapes consumed by `PaasContentAdapter` -
the fragment-building and rule logic in `OptiA11y.Core` is unchanged regardless of host.

The `RunAudit.cshtml` report view is styled to resemble the Optimizely admin interface and
supports client-side filtering and sorting of findings, with deep links back to the offending
property where the host's `IEditorLinkResolver` can resolve one.

## Rendered-style enrichment (optional)

By default, `color-contrast` and `text-readability` can only evaluate inline `style=""`
attributes, since `HtmlFragmentParser` performs pure HTML parsing with no browser or CSS engine.
Anything driven by external stylesheets, CSS classes, or theme styling is invisible to that path.
And five checks - `target-size`, `focus-indicator`, `motion`, `reflow`, and `text-spacing` - have
no inline-HTML signal to go on at all; they only exist because a real browser can measure a
rendered layout, focus an element, resize a viewport, and apply CSS overrides.

`OptiA11y.Rendering` is a separately packaged, opt-in slice that closes this gap using headless
Chromium via [Playwright](https://playwright.dev/dotnet/). For a content item's actual preview
URL, it:

- reads the *computed* CSS for every visible text node (contrast, readability, and whether the
  background is an image/gradient rather than a flat color);
- measures the rendered bounding box of every interactive element, and whether its appearance
  changes at all when focused (target size, focus indicator);
- scans for elements with an automatically-starting, non-stopping CSS animation (motion);
- resizes the viewport to 320px-equivalent and checks for horizontal overflow (reflow);
- injects the WCAG 1.4.12 reference text-spacing overrides and checks for clipped text (text spacing).

All of this is appended onto the `AuditDocument` as the same fragment shapes the inline-parsing
path would produce, before the rule engine runs - the rules themselves (`ColorContrastRule`,
`TextReadabilityRule`, `TargetSizeRule`, `FocusIndicatorRule`, `MotionRule`, `ReflowRule`,
`TextSpacingRule`) need no knowledge of where their fragments came from.

This is entirely opt-in and additive:

```csharp
services.AddOptiA11y();                 // existing rule set, unaffected
services.AddOptiA11yRenderedStyles();   // opt-in: registers PlaywrightRenderedStyleProvider

// Required: hosts must supply their own resolver so a preview URL can be built per content item.
// The default NullContentPreviewUrlResolver returns null, which skips rendered-style enrichment entirely.
services.AddSingleton<IContentPreviewUrlResolver, MyContentPreviewUrlResolver>();
```

Playwright's browser binary must be installed once per machine/build agent:

```powershell
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

If the browser isn't installed, or no preview URL resolver is registered, rendering fails soft and
the audit falls back to inline-style-only fragments (and the five rendered-only rules simply find
nothing to evaluate) — existing hosts see zero behavior change unless they explicitly opt in. See
[`src/OptiA11y.Rendering/README.md`](https://github.com/adayinthelifeofapro/OptiA11y/blob/master/src/OptiA11y.Rendering/README.md) for the full design
rationale and known limitations (best-effort text correlation, per-request browser latency,
host-supplied preview URL resolution, and the target-size/motion exceptions the underlying success
criteria carve out that this slice cannot fully verify).

## Non-goals for v1

- No crawler
- No accessibility score
- No automated fixing
- No publish blocking

## Slice order

1. **Audit a single content item** — adapter, engine, three rules (alt text quality, heading
   structure, link purpose), results view, deep link to the offending property.
2. **CMS 13 installable add-on** — self-registering module, editor menu entry, in-context editor
   panel, styled report view with filtering/sorting.
3. **Comprehensive rule expansion** — tables, media, form labels, buttons, iframes, deprecated
   elements, interactive attributes, list structure, language attributes, colour contrast, text
   readability.
4. **Rendered-style enrichment** (optional slice) — headless-browser computed-style capture for
   colour contrast and readability rules, layered on top of the inline-style-only foundation.
5. Issue register — persisted results across content, filterable, with status.
6. Dismissal with recorded reason — who, when, why. Auditable.
7. Publish warning — content event handler, warn only, severity configurable.
8. SaaS adapter — same engine, REST API source (scaffolded; not yet wired to a real tenant).

Persistence (EF Core, following the Stott Security pattern rather than DDS, for fewer assumptions
and a cleaner uninstall) arrives with the issue register in a later slice; the current slices
deliberately hold no persistence — `RunAuditHandler` returns results in memory only.

## Open verification items

1. Whether the Razor Class Library + menu provider add-on pattern continues to hold across future
   CMS 13 minor versions.
2. DDS availability — moot while there is no persistence, but determines whether the later EF Core
   choice in the issue register is a preference or a necessity.
3. Correlating rendered DOM text nodes (from `OptiA11y.Rendering`) back to a specific
   `SourceLocation` is currently best-effort (matched by trimmed visible text), which could produce
   ambiguous results on pages with duplicate identical text.

## Running the sample

```
dotnet run --project sample/OptiA11y.SampleSite
```

Then request `GET /optia11y/audit/page-home` to see findings against seeded content that
includes a nested block, exercising every rule and the deep-linking path.

## Packaging

`OptiA11y.Core`, `OptiA11y.Rendering`, `OptiA11y.Cms`, and `OptiA11y.Cms12` are packed and versioned
together (see each project's `<Version>`) since the CMS-facing packages depend on `Core`/`Rendering`
as NuGet packages rather than project references once installed into a host. `OptiA11y.Cms` targets
Optimizely CMS 13 (`EPiServer.Cms.Core` 13.x); `OptiA11y.Cms12` targets CMS 12
(`EPiServer.Cms.Core`/`EPiServer.CMS.UI.Core` 12.x) — install whichever matches your host's CMS
version, not both. To build and pack everything into a local feed:

```powershell
dotnet pack src\OptiA11y.Core\OptiA11y.Core.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Rendering\OptiA11y.Rendering.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Cms\OptiA11y.Cms.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Cms12\OptiA11y.Cms12.csproj -c Release -o .localfeed
```

Bump versions together when any shared code changes — a host resolving `OptiA11y.Cms`/`OptiA11y.Cms12`
against a stale cached `OptiA11y.Core`/`OptiA11y.Rendering` version will fail at runtime with a
`TypeLoadException` if the assemblies have drifted apart.

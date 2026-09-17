# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```powershell
dotnet build OptiA11y.slnx
dotnet test OptiA11y.slnx

# Single test project / single test class or method
dotnet test tests\OptiA11y.Core.Tests
dotnet test tests\OptiA11y.Core.Tests --filter "FullyQualifiedName~AltTextQualityRuleTests"
dotnet test tests\OptiA11y.Core.Tests --filter "FullyQualifiedName~HeuristicRulesNeverFailTests.AltTextQualityRule_NeverReportsFail_ForHeuristicJudgements"

# Run the sample host, then GET /optia11y/audit/page-home
dotnet run --project sample\OptiA11y.SampleSite

# Pack all shipped packages into the local feed (see "Versioning" below)
dotnet pack src\OptiA11y.Core\OptiA11y.Core.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Rendering\OptiA11y.Rendering.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Cms\OptiA11y.Cms.csproj -c Release -o .localfeed
dotnet pack src\OptiA11y.Cms12\OptiA11y.Cms12.csproj -c Release -o .localfeed

# One-time per machine, only if working on OptiA11y.Rendering against a real browser
pwsh src\OptiA11y.Cms\bin\Debug\net10.0\playwright.ps1 install chromium
```

`Directory.Build.props` sets `net10.0`, nullable, implicit usings, `TreatWarningsAsErrors=true`
and `EnforceCodeStyleInBuild=true` — a style violation is a build break, so match
`.editorconfig` (4 spaces, CRLF, Allman braces, `var`, no `this.` qualification).

## Architecture

Ports and adapters. The data flow for every audit is:

```
host content source → adapter → AuditDocument (ContentFragment[]) → RuleEngine → Finding[]
```

- **`src/OptiA11y.Core`** — the engine. Depends only on HtmlAgilityPack; **must never take an
  Optimizely/EPiServer dependency**. That constraint is what makes the whole rule set unit-testable
  with no CMS present (`tests/OptiA11y.Core.Tests` references Core alone).
- **`src/OptiA11y.Cms`** — Razor Class Library, the PaaS adapter and the installable CMS 13 add-on.
- **`src/OptiA11y.Saas`** — REST Content API adapter producing the same `AuditDocument` shape.
- **`src/OptiA11y.Rendering`** — optional headless-Chromium slice (see below).

Rules never see HTML, a DOM, or a CMS API — only typed fragments
(`Model/Fragments/*Fragment.cs`), each carrying a `SourceLocation`
(content reference + property name + nested block path + ordinal). `SourceLocation` is the
deep-linking payload; turning it into an editor URL is the host's job via `IEditorLinkResolver`,
which Core deliberately leaves unimplemented.

`HtmlFragmentParser` (`Core/Parsing`) is the **single** HTML→fragment entry point. PaaS, SaaS and
rendered-style enrichment all funnel through it, which is what makes rule behaviour identical
across content sources. `AdapterEquivalenceTests` in `tests/OptiA11y.Adapters.Tests` guards this —
if you add parsing anywhere else, that guarantee is gone.

### The Confidence invariant — the most important rule in the codebase

`Finding.Confidence` is `Fail` or `NeedsReview`. **A rule whose judgement depends on
natural-language quality or editorial intent must never emit `Fail`.** `Fail` is reserved for
deterministic structural facts (missing `alt` attribute, skipped heading level, contrast below
threshold). This is the mechanism that stops the tool manufacturing false assurance, and it is
enforced by `tests/OptiA11y.Core.Tests/Rules/HeuristicRulesNeverFailTests.cs`. Note `Severity` is a
separate axis: impact, not certainty.

The README states the same commitment externally: OptiA11y is editorial assistance, **not** a
compliance certification tool. Don't add features (scores, pass/fail certificates, publish
blocking) that undercut that — see "Non-goals for v1" in the README.

### Adding a rule

1. Add or reuse a fragment record in `Core/Model/Fragments/`, and emit it from `HtmlFragmentParser`.
2. Add `Core/Rules/<Name>/<Name>Rule.cs` implementing `IContentRule` (`RuleId`,
   `SuccessCriterion`, `Level`, `Evaluate`). Never throw for unusual content — unexpected shapes
   mean "nothing to flag".
3. Register it in `ServiceCollectionExtensions.AddOptiA11y` (`src/OptiA11y.Cms/Extensions`). Rules
   are individually DI-registered so hosts can add or drop them; `RuleEngine` is constructed from
   `IEnumerable<IContentRule>` and sorts findings deterministically by location.
4. Add the row to the rule catalogue table in `README.md` — it is the user-facing contract.
5. If the rule is heuristic, extend `HeuristicRulesNeverFailTests`.

### CMS 13 add-on self-registration

`OptiA11y.Cms` installs with zero host code. The wiring chain is worth knowing before changing any
of it:

- `Infrastructure/OptiA11yCmsModule.cs` — `[InitializableModule] IConfigurableModule` that calls
  `AddOptiA11y()`, registers `EPiServerPaasContentLoader` and registers the startup filter.
- `Infrastructure/OptiA11yStartupFilter.cs` — `IStartupFilter` that maps both endpoints, so hosts
  never call `MapRunAuditEndpoint()` themselves (the sample site does, because it has no EPiServer
  initialization pipeline).
- `Infrastructure/Navigation/OptiA11yAuditPanelPlugin.cs` + `RelativeUrlIFrameComponentAttribute.cs`
  — in-context editor panel in the assets pane. The shell's `IFrameContextComponent` appends the
  current editor context as an `id` query parameter and reloads on context change; that is why
  `RunAuditPanelEndpoint` reads `id` (falling back to `contentLink`).

Routes: `/optia11y/audit/{contentReference}` (JSON) and `/optia11y/audit/panel` (renders
`Features/RunAudit/Views/RunAudit.cshtml` directly through `IRazorViewEngine`, avoiding any
requirement that the host register MVC controllers).

`EPiServerPaasContentLoader` maps `IContent`/`XhtmlString`/`ContentArea` into the CMS-agnostic
`PaasContentNode`/`PaasProperty` records. Keep the EPiServer types confined to that loader —
`PaasContentAdapter` and everything below it stay CMS-free. (The XML doc on `PaasContentNode`
claiming EPiServer.CMS.Core is unresolvable is stale; it restores fine and the real loader exists.)

### Rendered-style enrichment (optional)

`OptiA11y.Rendering` is opt-in and purely additive: `RunAuditHandler` appends
`ColorContrastFragment`/`TextStyleFragment` built from real computed CSS *before* the engine runs,
so `ColorContrastRule`/`TextReadabilityRule` need no knowledge of it. It is skipped entirely unless
the host registers both `AddOptiA11yRenderedStyles()` and its own `IContentPreviewUrlResolver`
(the default resolver returns null). **`PlaywrightRenderedStyleProvider` must fail soft** — a
missing browser binary or unreachable page returns an empty list and the audit falls back to
inline-style-only fragments. Never let it throw into an audit.

### No persistence yet

`RunAuditHandler` returns results in memory only. The issue register (slice 5) will introduce
EF Core persistence, following the Stott Security pattern rather than DDS. Don't add persistence
ad hoc before then.

## Versioning

`OptiA11y.Core`, `OptiA11y.Rendering` and `OptiA11y.Cms` each carry their own `<Version>` in their
csproj (which overrides the `Directory.Build.props` default) and ship together, because
`OptiA11y.Cms` resolves the other two as NuGet packages once installed in a host. **Bump all three
together on any change** — a host resolving `OptiA11y.Cms` against a stale cached
`OptiA11y.Core`/`OptiA11y.Rendering` fails at runtime with `TypeLoadException`.

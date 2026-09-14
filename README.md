# OptiA11y

OptiA11y is editorial accessibility assistance for Optimizely content authors. It is **not a
compliance certification tool**. A clean audit result means OptiA11y found no issues it was able
to detect — it is not a legal position, and it should never be presented to a client, auditor, or
regulator as proof of WCAG or ADA conformance. Treat OptiA11y as a second pair of eyes for
editors, not as a substitute for a professional accessibility audit.

## What it does

OptiA11y evaluates content **at the property level** — alt text quality, link purpose, heading
structure, table semantics, language of parts, and document link labelling — without rendering a
page or running a browser. This is the differentiated editorial value: it is the analysis
Siteimprove and similar crawler-based tools handle least well, and it works identically whether
content lives on PaaS (via `IContentLoader`) or SaaS (via the Content Delivery API).

A later slice will add axe-core DOM analysis (colour contrast, focus order, landmarks) as an
enrichment on top of this foundation, not as the starting point.

## The Confidence model — the honesty mechanism

Every finding carries a `Confidence`:

- **`Fail`** — a deterministic, unambiguous violation (a missing alt attribute, a skipped heading
  level, an empty heading). Reserved for structural facts a rule can be certain about.
- **`NeedsReview`** — a heuristic judgement (alt text quality, link text descriptiveness). The
  tool cannot know editorial intent, so it flags something worth a human's attention rather than
  asserting a defect.

Heuristic rules must never report `Fail`. This distinction is what stops OptiA11y from
manufacturing false assurance, and it is enforced by a dedicated test
(`HeuristicRulesNeverFailTests`) in `tests/OptiA11y.Core.Tests`.

## Architecture

Ports and adapters at the core, vertical slices at the CMS layer:

```
src/
  OptiA11y.Core/     Rules engine, model, and content rules. No Optimizely dependency.
  OptiA11y.Cms/       Razor Class Library: PaaS adapter, RunAudit feature, DI wiring.
  OptiA11y.Saas/      REST API adapter, isolated SaaS response mapping.
tests/
  OptiA11y.Core.Tests/       Rule fixtures, no CMS dependency.
  OptiA11y.Adapters.Tests/   PaaS/SaaS adapter equivalence and endpoint tests.
sample/
  OptiA11y.SampleSite/       Seeded content with deliberate accessibility problems.
```

`OptiA11y.Core` has zero Optimizely dependencies and is fully unit-testable without a CMS
present. Rules operate against a normalised `AuditDocument` made of typed fragments (image, link,
heading, table, text, media), each carrying a `SourceLocation` (content reference, property name,
nested block path, ordinal) that identifies exactly where the finding came from — this is the
foundation for deep-linking a finding back to the offending property, including inside nested
blocks in a content area.

## CMS 13 add-on

`OptiA11y.Cms` is a true installable add-on for Optimizely CMS 13 - no code needs to be added to
the host solution. Install the package and it self-registers via:

- `Infrastructure/OptiA11yCmsModule.cs` - an `IConfigurableModule` that registers all OptiA11y
  services (rule set, rule engine, RunAudit handler, and a real `IContentLoader`-backed
  `EPiServerPaasContentLoader`) into the host's DI container automatically during EPiServer's
  initialization pipeline.
- `Infrastructure/Navigation/OptiA11yNavigation.cs` - an `IMenuProvider` implementation decorated
  with `[MenuProvider]`, which the CMS shell discovers automatically and uses to add the
  "Accessibility audit" entry to the editor menu.
- `Infrastructure/OptiA11yStartupFilter.cs` - an `IStartupFilter`, registered by
  `OptiA11yCmsModule`, that maps the RunAudit endpoint into the host's ASP.NET Core pipeline
  automatically. Hosts do not need to call `MapRunAuditEndpoint()` themselves.

`EPiServerPaasContentLoader` maps `IContent`/`XhtmlString`/`ContentArea` (including nested blocks)
into the CMS-agnostic `PaasContentNode`/`PaasProperty` shapes consumed by `PaasContentAdapter` -
the fragment-building and rule logic in `OptiA11y.Core` is unchanged from slice one.

## Non-goals for v1

- No crawler
- No headless browser
- No accessibility score
- No automated fixing
- No publish blocking

## Slice order

1. **Audit a single content item** (this slice) — adapter, engine, three rules (alt text quality,
   heading structure, link purpose), results view, deep link to the offending property.
2. Issue register — persisted results across content, filterable, with status.
3. Dismissal with recorded reason — who, when, why. Auditable.
4. Publish warning — content event handler, warn only, severity configurable.
5. SaaS adapter — same engine, REST API source (scaffolded in this slice; not yet wired to a
   real tenant).
6. axe-core DOM enrichment — client-side in the preview iframe, results posted back to the same
   store.

Slice one deliberately has **no persistence**: `RunAuditHandler` returns results in memory only.
Persistence (EF Core, following the Stott Security pattern rather than DDS, for fewer assumptions
and a cleaner uninstall) arrives with the issue register in slice two.

## Open verification items

Three assumptions from the original plan remain unconfirmed and should be spiked before deeper
CMS-13-specific investment:

1. The CMS 13 editor extensibility model for in-context panels (React has replaced Dojo; the
   supported extension surface is unknown). Slice one's standalone `RunAudit` view avoids this
   entirely, and is the safe route regardless of how this resolves.
2. Whether the Razor Class Library + menu provider add-on pattern still holds in CMS 13.
3. DDS availability — moot for slice one (no persistence), but determines whether the later EF
   Core choice in the issue register is a preference or a necessity.

## Running the sample

```
dotnet run --project sample/OptiA11y.SampleSite
```

Then request `GET /optia11y/audit/page-home` to see findings against seeded content that
includes a nested block, exercising every slice-one rule and the deep-linking path.

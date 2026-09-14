# OptiA11y.Core

The accessibility rules engine behind [OptiA11y](https://github.com/adayinthelifeofapro/OptiA11y),
editorial accessibility assistance for Optimizely content authors. **Not a compliance
certification tool** — a clean audit result means OptiA11y found no issues it was able to detect,
not proof of WCAG or ADA conformance.

This package has **zero Optimizely/CMS dependencies** (only [HtmlAgilityPack](https://html-agility-pack.net/))
and is fully unit-testable on its own. It defines the ports-and-adapters core that every host
adapter (PaaS, SaaS) and the optional rendered-style slice funnel into:

```
host content source → adapter → AuditDocument (ContentFragment[]) → RuleEngine → Finding[]
```

## What's in this package

- **`Model`** — `AuditDocument`, `Finding`, `SourceLocation`, and the typed `ContentFragment`
  shapes (image, link, heading, table, text, media, form field, button, iframe, and more) that
  every rule operates against. Rules never see raw HTML or a DOM.
- **`Parsing.HtmlFragmentParser`** — the single, shared entry point that turns raw HTML into
  fragments. Every host adapter funnels through this, so rule behaviour is identical regardless
  of where the content came from.
- **`Rules`** — over 30 independently DI-registrable `IContentRule` implementations covering alt
  text, headings, links, tables, forms, buttons, ARIA usage, language, colour contrast, reading
  level, and more. Each declares its own WCAG success criterion and level.
- **`RuleEngine`** — runs a set of rules against an `AuditDocument` and returns findings ordered
  deterministically by location.

## The Confidence model — the honesty mechanism at the centre of this package

Every `Finding` carries a `Confidence`:

- **`Fail`** — a deterministic, unambiguous violation (a missing alt attribute, a skipped heading
  level, a contrast ratio below threshold). Reserved for structural facts a rule can be certain
  about.
- **`NeedsReview`** — a heuristic judgement (alt text quality, link text descriptiveness, reading
  level). The tool cannot know editorial intent, so it flags something worth a human's attention
  rather than asserting a defect.

Heuristic rules must never report `Fail` — this is what stops the tool from manufacturing false
assurance.

## Using this package directly

Most consumers will want [`OptiA11y.Cms`](https://www.nuget.org/packages/OptiA11y.Cms) (the
Optimizely CMS 13 installable add-on) rather than this package directly. Reach for
`OptiA11y.Core` on its own if you're building a custom adapter for a content source `OptiA11y.Cms`
doesn't cover — implement `HtmlFragmentParser`-based fragment extraction for your source, register
the rules you want via `RuleEngine`, and evaluate an `AuditDocument`.

See the [main repository README](https://github.com/adayinthelifeofapro/OptiA11y) for the full
rule catalogue, architecture overview, and versioning notes (this package, `OptiA11y.Rendering`,
and `OptiA11y.Cms` ship together and must be kept in sync).

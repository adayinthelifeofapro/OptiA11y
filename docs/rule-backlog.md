# Rule backlog — comprehensive accessibility rule expansion

This document is the working register for the rule-expansion effort. It lists every rule proposed
beyond the catalogue already shipped in [README.md](../README.md), together with the WCAG success
criterion it maps to, its conformance level, the `Confidence` it is permitted to report, and the
batch it belongs to.

## How to read the Confidence column

The rules in this backlog are bound by the same honesty mechanism as the existing catalogue:

- **`Fail`** — the rule can establish a deterministic, unambiguous fact from the markup alone
  (an attribute is absent, an id does not resolve, a role is not in the ARIA vocabulary).
- **`NeedsReview`** — the rule is making a heuristic judgement about editorial intent, natural
  language, or visual design. It must never report `Fail`, and it must be added to
  `HeuristicRulesNeverFailTests` in `tests/OptiA11y.Core.Tests`.

A rule listed as `Fail + NeedsReview` has both a deterministic branch and a heuristic branch, in
the same shape as the existing `alt-text-quality` and `heading-structure` rules.

## Property-level vs rendered-only

- **Property-level** rules run against fragments produced by `HtmlFragmentParser` and require no
  browser. They work identically on PaaS and SaaS.
- **Rendered-only** rules are still implemented as `IContentRule` classes in
  `src/OptiA11y.Core/Rules` (matching the existing `target-size`, `focus-indicator`, `motion`,
  `reflow`, and `text-spacing` rules), but they consume fragments that only the optional
  `OptiA11y.Rendering` slice can produce. Without that slice registered, the fragments are absent
  and the rule yields nothing — it must never guess.

## Batch 1 — Images and non-text content

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `image-of-text` | 1.4.5 | AA | NeedsReview | Alt text that reads as a sentence or heading, suggesting text baked into the image |
| `decorative-image-misuse` | 1.1.1 | A | NeedsReview | `alt=""` on an image that is linked or figure-wrapped, i.e. probably informative |
| `figure-caption-mismatch` | 1.1.1 | A | NeedsReview | `<figcaption>` identical to the image's `alt`, causing duplicate announcement |
| `long-alt-text` | 1.1.1 | A | NeedsReview | Alt text beyond a sensible character budget with no long description |
| `complex-image-description` | 1.1.1 | A | NeedsReview | Chart/graph/diagram keywords in alt or filename with only short alt text |

## Batch 2 — Structure and relationships

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `meaningful-sequence` | 1.3.2 | A | NeedsReview | Inline float/absolute positioning that may diverge visual order from DOM order |
| `layout-table` | 1.3.1 | A | NeedsReview | A table with no headers or caption, used for layout |
| `list-misuse` | 1.3.1 | A | Fail | An empty `<ul>`/`<ol>`, or non-`<li>` element children directly inside a list |
| `definition-list-structure` | 1.3.1 | A | Fail | `<dl>` with `<dd>` before any `<dt>`, or orphaned terms/descriptions |
| `blockquote-misuse` | 1.3.1 | A | NeedsReview | `<blockquote>` used purely for indentation |
| `duplicate-id` | 4.1.1 | A | Fail | Repeated `id` values within a single property |
| `heading-length` | 2.4.6 | AA | NeedsReview | Headings far too long to function as labels |
| `label-quality` | 2.4.6 | AA | NeedsReview | Non-descriptive form labels such as "Field 1" |
| `emphasis-misuse` | 1.3.1 | A | NeedsReview | `<b>`/`<i>` wrapping whole paragraphs instead of `<strong>`/`<em>` or a heading |
| `superscript-subscript-misuse` | 1.3.1 | A | NeedsReview | `<sup>`/`<sub>` wrapping long runs of text |

## Batch 3 — Links, navigation, and titles

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `same-text-different-destination` | 2.4.4 | A | NeedsReview | Identical link text pointing at different URLs |
| `same-destination-different-text` | 2.4.4 | A | NeedsReview | Different link text pointing at the same URL |
| `raw-url-link-text` | 2.4.4 | A | NeedsReview | Link text that is a bare URL |
| `new-window-link` | 3.2.5 | AAA | NeedsReview | `target="_blank"` with no warning in the accessible name |
| `empty-anchor` | 4.1.2 | A | Fail | An `<a href>` with no content and no accessible name |
| `adjacent-duplicate-links` | 2.4.4 | A | NeedsReview | An image link immediately followed by a text link to the same URL |
| `skip-link-target` | 2.4.1 | A | Fail | An in-page `href="#id"` whose target does not exist in the same property |
| `meta-refresh` | 2.2.1 | A | Fail | `<meta http-equiv="refresh">` in content markup |
| `page-title-uniqueness` | 2.4.2 | A | NeedsReview | A content name identical to a sibling's |

**Implementation status:** `same-text-different-destination`, `raw-url-link-text`, and
`empty-anchor` were not implemented as separate rules — they duplicate checks already shipped in
`link-purpose` (bare-URL and duplicate-text-different-target detection) and `link-name`
(no-accessible-name-at-all detection) respectively. `same-destination-different-text`,
`new-window-link`, and `adjacent-duplicate-links` shipped as `NeedsReview` rules.
`skip-link-target` shipped as `NeedsReview` rather than `Fail` — like `broken-aria-reference`,
resolution is scoped to a single property's markup, so an unresolved anchor target is a signal,
not proof, since the target could legitimately live in the page template. `meta-refresh` shipped
as `Fail`, since the tag's presence is a deterministic structural fact. `page-title-uniqueness` is
deferred: `AuditDocument` models exactly one content item, with no mechanism today for a rule to
see sibling content names, so this rule cannot be implemented honestly without an architectural
change (e.g. injecting a sibling-name repository into the rule engine).

## Batch 4 — Forms and input

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `placeholder-as-label` | 3.3.2 | A | Fail | A field whose only accessible name comes from `placeholder` |
| `required-field-indication` | 3.3.2 | A | NeedsReview | A visually required field (asterisk) with no `required`/`aria-required` |
| `error-identification` | 3.3.1 | A | NeedsReview | Error text not programmatically associated with its field |
| `form-instructions` | 3.3.2 | A | NeedsReview | Format-constrained fields with no associated instruction text |
| `select-option-quality` | 1.3.1 | A | NeedsReview | A `<select>` whose first option acts as a label, or has empty option text |
| `readonly-disabled-misuse` | 4.1.2 | A | NeedsReview | `aria-disabled` without a real disabled state |
| `redundant-entry` | 3.3.7 | A | NeedsReview | Repeated identical field labels within one form |
| `input-type-appropriateness` | 1.3.5 | AA | NeedsReview | Email/phone/date-looking fields typed as plain `text` |

Batch 4 implementation notes: `readonly-disabled-misuse` was implemented as "disabled/aria-disabled
field that also looks required (required attribute or visual asterisk)" rather than "aria-disabled
without a real disabled state" - the latter is already deterministically knowable (`disabled` is
either present or not) and wouldn't need `NeedsReview`, whereas a disabled-but-required field is a
believable sign of stale conditional-display logic and needs a human look. `redundant-entry` was
implemented as "field whose label/placeholder contains a confirmation keyword (confirm/verify/
re-enter) with no autocomplete" rather than "repeated identical field labels within one form" -
`AuditDocument`/`FormFieldFragment` don't currently group fields by enclosing `<form>`, so comparing
labels across the whole document risked false positives from unrelated fields sharing generic label
text; the keyword heuristic targets the same WCAG 3.3.7 redundant-entry concern without that
architectural dependency.

## Batch 5 — ARIA and semantics

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `aria-required-children` | 1.3.1 | A | Fail | Roles with required owned elements (`list`, `tablist`, `menu`) missing them |
| `aria-required-attributes` | 4.1.2 | A | Fail | Roles missing mandatory attributes (e.g. `checkbox` without `aria-checked`) |
| `aria-allowed-attribute` | 4.1.2 | A | Fail | A valid ARIA attribute applied to a role that does not support it |
| `redundant-role` | 4.1.2 | A | NeedsReview | An explicit role duplicating the element's native semantics |
| `aria-hidden-focusable` | 4.1.2 | A | Fail | `aria-hidden="true"` containing focusable descendants |
| `presentation-role-conflict` | 1.3.1 | A | Fail | `role="presentation"`/`none` on a focusable element or one with global ARIA |
| `landmark-structure` | 1.3.1 | A | NeedsReview | Content-embedded `main`/`banner` roles, or duplicate unlabelled landmarks |
| `live-region-misuse` | 4.1.3 | AA | NeedsReview | `aria-live`/`role="status"` on static content, or an invalid politeness value |

Batch 5 implementation notes: all 8 rules were implemented as scoped in the table above, each backed
by a new `AriaSemanticsFragment`, `LandmarkFragment`, or `LiveRegionFragment` produced by
`HtmlFragmentParser`. `aria-required-children`, `aria-required-attributes`, and
`aria-allowed-attribute` are deliberately scoped to a small, well-established subset of the ARIA
specification's required-owned-elements/required-attributes/attribute-allowance tables (a handful
of common roles and the four widget-state attributes `aria-checked`/`aria-selected`/
`aria-expanded`/`aria-pressed`) rather than claiming full spec coverage - this keeps the Fail
confidence honest, since the ARIA spec is a fixed vocabulary but this codebase only encodes part
of it. `redundant-role` is `NeedsReview` rather than `Fail` because some authors deliberately keep
a redundant explicit role for older assistive technology compatibility. `landmark-structure` and
`live-region-misuse` are `NeedsReview` because the parser can only see markup, not runtime
behaviour or overall page template structure, so both "is this really a template-owned landmark"
and "does this live region actually update" are judgement calls.

## Batch 6 — Language, readability, and editorial quality

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `abbreviation-expansion` | 3.1.4 | AAA | NeedsReview | Repeated acronyms with no `<abbr title>` or inline expansion |

| `unusual-words` | 3.1.3 | AAA | NeedsReview | Jargon and idiom candidates with no glossary link |
| `pronunciation-ambiguity` | 3.1.6 | AAA | NeedsReview | Heteronyms and ambiguous terms flagged for author judgement |
| `unicode-styled-text` | 1.3.1 | A | Fail | Mathematical-alphanumeric or fullwidth "fancy font" characters |
| `emoji-overuse` | 1.1.1 | A | NeedsReview | Repeated decorative emoji runs, or emoji used as bullets |
| `ascii-art` | 1.1.1 | A | NeedsReview | Punctuation-dense lines that render as art or dividers |
| `whitespace-formatting` | 1.3.1 | A | NeedsReview | Non-breaking spaces or repeated spaces used for layout |
| `line-break-misuse` | 1.3.1 | A | NeedsReview | Consecutive `<br>` used to fake paragraphs |
| `link-text-language` | 3.1.2 | AA | NeedsReview | Link text in a different writing system with no `lang` |

`text-readability` is extended (rather than duplicated) to cover ALL-CAPS headings.

Batch 6 implementation notes: all 9 rules (the 8 listed above, plus `text-readability`'s
pre-existing ALL-CAPS check) were implemented as scoped. `abbreviation-expansion`, `unusual-words`,
`pronunciation-ambiguity`, `unicode-styled-text`, `emoji-overuse`, and `ascii-art` operate directly
on the existing `TextFragment` produced by `HtmlFragmentParser` - no new fragment was needed.
`line-break-misuse` is backed by a new `LineBreakRunFragment`, built by scanning for runs of 2+
consecutive `<br>` elements. `link-text-language` reuses the same non-Latin-script detection regex
as `language-of-parts`, applied to a new `LinkFragment.LanguageCode` property populated from the
nearest declared `lang` attribute. `unicode-styled-text` is `Fail` rather than `NeedsReview`
because it's restricted to two small, fixed Unicode blocks (Mathematical Alphanumeric Symbols and
Halfwidth/Fullwidth Forms) that have no legitimate use in body text - a deterministic structural
fact rather than a judgement call. `unusual-words` and `pronunciation-ambiguity` intentionally use
small, curated, non-exhaustive word lists (documented in the rule's own summary) to keep the
`NeedsReview` confidence honest about their limited scope.

## Batch 7 — Time, motion, and media

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `blinking-content` | 2.2.2 | A | Fail | Inline animation/blink declarations on content elements |
| `timed-content` | 2.2.1 | A | NeedsReview | Text describing a session or countdown limit |
| `media-transcript-quality` | 1.2.1 | A | NeedsReview | A "transcript" link pointing at the media file itself, or with weak text |
| `sign-language` | 1.2.6 | AAA | NeedsReview | Prerecorded video with no sign-language track reference |
| `extended-audio-description` | 1.2.7 | AAA | NeedsReview | Description-heavy video with only a standard AD track |

| `media-alternative` | 1.2.3 | A | NeedsReview | Video with neither audio description nor a full text alternative |
| `flashing-content` | 2.3.1 | A | NeedsReview | Media authored with rapid-flash indicators in its name or metadata |

Batch 7 implementation notes: all 7 rules were implemented as scoped in the table above.
`blinking-content` is backed by a new `BlinkingContentFragment`, built for the deprecated
`<blink>`/`<marquee>` tags or an inline `text-decoration: blink`/`animation` style naming
"blink" - detecting the declaration is a deterministic structural fact, so it reports `Fail`
rather than `NeedsReview`. `timed-content` reuses the existing `TextFragment` with a phrasing
heuristic over session/countdown language. `media-transcript-quality`, `sign-language`,
`extended-audio-description`, and `media-alternative` all extend `MediaFragment` with new
`HasSignLanguageTrack`, `HasExtendedDescriptionTrack`, `TranscriptHref`, and
`TranscriptLinkText` fields, populated by `HtmlFragmentParser.BuildMediaFragment` from
`<track>` elements and the nearest sibling "transcript" link - no new fragment type was
needed for these four. `flashing-content` adds a `HasFlashIndicator` field to the same
fragment, set from a weak textual heuristic (the media source URL or class/data attributes
mentioning flash/strobe/flicker); this is intentionally distinct from the rendered-only
`flash-threshold` rule in Batch 9, which measures actual sampled luminance rather than
guessing from naming conventions.

## Batch 8 — Colour and presentation


| --- | --- | --- | --- | --- |
| `use-of-color` | 1.4.1 | A | NeedsReview | Instructions referencing colour alone |
| `link-distinguishability` | 1.4.1 | A | NeedsReview | An inline link distinguished from body text by colour only |
| `non-text-contrast` | 1.4.11 | AA | Fail | Inline-styled borders or icon fills below 3:1 against their background |
| `contrast-enhanced` | 1.4.6 | AAA | Fail | Text below the AAA 7:1 / 4.5:1 thresholds |
| `line-length` | 1.4.8 | AAA | NeedsReview | Inline-styled width producing very long lines |
| `line-spacing` | 1.4.8 | AAA | NeedsReview | Inline `line-height` below 1.5 on body copy |
| `background-image-text` | 1.4.5 | AA | NeedsReview | Inline `background-image` on a text-bearing element |

The image/gradient-background branch of `color-contrast` stays inside that rule rather than being
split into a separate `text-in-image-contrast` rule, so one authoring mistake produces one finding.

Batch 8 implementation notes: all 7 rules were implemented as scoped in the table above.
`use-of-color` reuses `TextFragment` with a phrasing heuristic over color-only references
(e.g. "shown in red"). `link-distinguishability` extends `LinkFragment` with
`HasExplicitColorStyle` and `RemovesUnderline`, flagging when both are set - a link with an
explicit color but its underline intact isn't flagged, since the underline is still a
non-color cue. `line-length`, `line-spacing`, and `background-image-text` all extend
`TextStyleFragment` with `WidthPx`, `LineHeight`, and `HasBackgroundImage` rather than adding
new fragment types, matching the "extend, don't duplicate" pattern from Batch 6/7.
`contrast-enhanced` reuses `ColorContrastFragment` outright, applying the stricter AAA
7:1/4.5:1 thresholds instead of AA's 4.5:1/3:1 - the same deterministic ratio, a different bar,
so no parser change was needed. `non-text-contrast` is backed by a new
`NonTextContrastFragment`, computed from an element's `border-color`/`fill` against the
nearest ancestor's explicit `background-color`; when no ancestor sets one, the fragment
documents an explicit white-default assumption, and the rule reports that specific case as
`NeedsReview` rather than `Fail` (since the assumption itself is unverified), while a ratio
computed against an ancestor's genuinely authored background is a deterministic `Fail`.

## Batch 9 — Rendered-only rules

These require `OptiA11y.Rendering` to be registered. Each yields nothing when its fragment is
absent.

| Rule ID | SC | Level | Confidence | What it checks |
| --- | --- | --- | --- | --- |
| `keyboard-trap` | 2.1.2 | A | Fail | Tab cycling that cannot escape a component |
| `focus-order` | 2.4.3 | A | NeedsReview | DOM tab order diverging from visual layout order |
| `focus-not-obscured` | 2.4.11 | AA | Fail | A focused element overlapped by sticky headers or footers |
| `focus-appearance` | 2.4.13 | AAA | NeedsReview | A focus indicator below the AAA appearance threshold |
| `keyboard-operable` | 2.1.1 | A | Fail | Click-handler elements not reachable or activatable by keyboard |
| `hover-focus-content` | 1.4.13 | AA | NeedsReview | Hover/focus content that is not dismissible, hoverable, or persistent |
| `orientation-lock` | 1.3.4 | AA | Fail | Layout that breaks or locks under a rotated viewport |
| `resize-text` | 1.4.4 | AA | Fail | Content loss or clipping at 200% text zoom |
| `sticky-obstruction` | 1.4.10 | AA | NeedsReview | Sticky elements consuming an excessive share of a small viewport |
| `flash-threshold` | 2.3.1 | A | NeedsReview | Sampled luminance changes exceeding the general flash threshold |
| `pointer-target-spacing` | 2.5.8 | AA | NeedsReview | Adjacent targets with insufficient spacing between them |
| `pointer-gestures` | 2.5.1 | A | NeedsReview | Components requiring multipoint or path-based gestures |
| `dragging-movements` | 2.5.7 | AA | NeedsReview | Drag-only interactions with no click alternative |
| `label-in-name` | 2.5.3 | A | Fail | Visible label text not contained in the accessible name |
| `status-messages` | 4.1.3 | AA | NeedsReview | Dynamically inserted status content with no live region |
| `bypass-blocks` | 2.4.1 | A | Fail | No skip link, landmark, or heading route past repeated navigation |
| `landmark-completeness` | 1.3.1 | A | NeedsReview | Missing `main`, content outside all landmarks, duplicate unlabelled landmarks |
| `heading-in-viewport-order` | 1.3.2 | A | NeedsReview | Rendered heading order diverging from DOM heading order |

`reflow` is extended with horizontal-scroll detection at 320 CSS px, and `motion` with
`prefers-reduced-motion` handling, rather than adding near-duplicate rules.

Batch 9 implementation notes: all 18 rules were implemented as scoped in the table above.
`RenderedElementDiagnostics` and `RenderedPageDiagnostics` were extended with the per-element and
page-level facts these rules need (e.g. `HasClickHandlerWithoutKeyboardAccess`, `AccessibleName`,
`IsObscuredWhenFocused`, `HasKeyboardTrap`, `HasMainLandmark`), captured by
`PlaywrightRenderedStyleProvider` via two new Playwright passes — `StructuralPassScript` (focus
order, bypass blocks, landmark completeness, heading order, status messages, flash timing) and
`StickyPassScript` (sticky viewport consumption and focused-element obstruction) — alongside a
dedicated `DetectKeyboardTrapAsync` helper that drives real Tab key presses rather than inferring a
trap heuristically. Each rule consumes its own new fragment type (18 in total, one per rule) built
by `RenderedStyleFragmentBuilder`, keeping rule inputs explicit and traceable. Confidence follows
the same honesty split as every other batch: `keyboard-trap`, `focus-not-obscured`,
`keyboard-operable`, `orientation-lock`, `resize-text`, `label-in-name`, and `bypass-blocks` report
`Fail` because they are backed by deterministic, measured facts (a real Tab cycle that cannot
escape, a measured overlap, a click handler with no keyboard path, and so on); the remaining 11
rules report `NeedsReview` because they rest on a heuristic judgement (an appearance threshold, a
timing-based flash-risk signal rather than sampled luminance, or a structural pattern that is
usually but not always a problem) and are covered by `HeuristicRulesNeverFailTests`. `flash-threshold`
in particular is deliberately a timing heuristic, not true luminance sampling, to avoid overclaiming
what a bounded browser pass can verify.

## Explicitly out of scope

These success criteria are site-wide rather than content-item-level. OptiA11y audits a single
content item, so it cannot answer them honestly and deliberately does not try:

- **2.4.5 Multiple ways (AA)** — requires knowing every route to a page across the site.
- **3.2.3 Consistent navigation (AA)** — requires comparing navigation across multiple pages.
- **3.2.4 Consistent identification (AA)** — requires comparing component naming across pages.

Reporting a pass or fail on these from a single content item would manufacture exactly the false
assurance the Confidence model exists to prevent.

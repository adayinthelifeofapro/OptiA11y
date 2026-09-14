using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.HeadingStructure;

/// <summary>
/// Evaluates heading structure: skipped levels, multiple H1s, empty headings, a missing H1, and
/// headings so long they read as body copy. The first three are deterministic structural facts,
/// not editorial judgements, so they may legitimately report <see cref="Confidence.Fail"/>.
///
/// The missing-H1 and overlong-heading checks report <see cref="Confidence.NeedsReview"/>
/// instead: a CMS page's H1 very often comes from the page template rather than from the content
/// being audited here, so "no H1 in this content" is not proof of a missing H1 on the rendered
/// page - and "is this heading actually too long" is a judgement call about the content, not a
/// structural fact.
/// </summary>
public sealed class HeadingStructureRule : IContentRule
{
    private const int MaximumReasonableHeadingLength = 120;

    public string RuleId => "heading-structure";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var headings = document.Get<HeadingFragment>().ToList();

        if (headings.Count == 0 || headings.All(h => h.Level != 1))
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Info,
                Confidence.NeedsReview,
                document.Fragments.Count > 0 ? document.Fragments[0].Location : SourceLocation.OnProperty(document.ContentReference, string.Empty),
                "No H1 was found in this content. If the page template doesn't render one either, add a single top-level heading.");
        }

        foreach (var heading in headings)
        {
            if (string.IsNullOrWhiteSpace(heading.Text))
            {
                yield return Fail(heading, "This heading has no text content. Empty headings confuse screen reader navigation.");
            }
            else if (heading.Text.Length > MaximumReasonableHeadingLength)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    heading.Location,
                    $"This heading is {heading.Text.Length} characters long, which reads more like body copy than a heading. Consider shortening it and moving detail into a paragraph.");
            }
        }

        var h1Count = headings.Count(h => h.Level == 1);
        if (h1Count > 1)
        {
            foreach (var heading in headings.Where(h => h.Level == 1).Skip(1))
            {
                yield return Fail(heading, "This page has more than one H1. There should be a single top-level heading per page.");
            }
        }

        int? previousLevel = null;
        foreach (var heading in headings)
        {
            if (previousLevel.HasValue && heading.Level > previousLevel.Value + 1)
            {
                yield return Fail(
                    heading,
                    $"This heading jumps from level {previousLevel.Value} to level {heading.Level}, skipping intermediate levels. Headings should descend one level at a time.");
            }

            previousLevel = heading.Level;
        }
    }

    private Finding Fail(HeadingFragment heading, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        heading.Location,
        message);
}

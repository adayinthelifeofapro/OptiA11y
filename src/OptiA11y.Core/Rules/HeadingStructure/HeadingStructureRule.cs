using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.HeadingStructure;

/// <summary>
/// Evaluates heading structure: skipped levels, multiple H1s, and empty headings.
/// All three checks here are deterministic structural facts, not editorial judgements,
/// so they may legitimately report <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class HeadingStructureRule : IContentRule
{
    public string RuleId => "heading-structure";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var headings = document.Get<HeadingFragment>().ToList();
        if (headings.Count == 0)
        {
            yield break;
        }

        foreach (var heading in headings)
        {
            if (string.IsNullOrWhiteSpace(heading.Text))
            {
                yield return Fail(heading, "This heading has no text content. Empty headings confuse screen reader navigation.");
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

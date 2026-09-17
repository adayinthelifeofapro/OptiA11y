using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SameDestinationDifferentText;

/// <summary>
/// Flags links that point at the same destination but use different visible text. This is a
/// judgement call about whether the difference is meaningful (e.g. "Read the report" vs.
/// "Download PDF" both linking to the same file may be fine) or confusing (users who navigate
/// by link list may not realise two entries lead to the same place), so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class SameDestinationDifferentTextRule : IContentRule
{
    public string RuleId => "same-destination-different-text";

    public string SuccessCriterion => "2.4.4";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var links = document.Get<LinkFragment>().ToList();

        foreach (var group in links.GroupBy(l => l.Href, StringComparer.OrdinalIgnoreCase))
        {
            if (group.Key.Length == 0)
            {
                continue;
            }

            var distinctTexts = group.Select(l => l.Text.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (distinctTexts.Count > 1)
            {
                foreach (var link in group)
                {
                    yield return new Finding(
                        RuleId,
                        SuccessCriterion,
                        Level,
                        Severity.Minor,
                        Confidence.NeedsReview,
                        link.Location,
                        $"This destination is linked to with {distinctTexts.Count} different link texts on this content. Consider using consistent text for the same destination so users recognise it's the same link.");
                }
            }
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.HeadingLength;

/// <summary>
/// Flags headings so long they can no longer function as a scannable label — the entire point
/// of a heading is to let a screen reader user jump the document structure and know at a glance
/// what a section is about. Where the line between "long but fine" and "too long" falls is a
/// judgement call, so this rule reports <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class HeadingLengthRule : IContentRule
{
    private const int MaxHeadingLength = 100;

    public string RuleId => "heading-length";

    public string SuccessCriterion => "2.4.6";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var heading in document.Get<HeadingFragment>())
        {
            if (heading.Text.Length > MaxHeadingLength)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    heading.Location,
                    $"This heading is {heading.Text.Length} characters long, which is too long to function as a scannable label. Consider a shorter heading, moving detail into the body text below it.");
            }
        }
    }
}

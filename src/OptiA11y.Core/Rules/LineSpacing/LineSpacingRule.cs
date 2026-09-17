using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LineSpacing;

/// <summary>
/// Flags an inline-styled text block with a <c>line-height</c> below the AAA-recommended 1.5x
/// (WCAG 1.4.8). Detecting the declared value is deterministic, but whether the resulting spacing
/// actually harms readability for this particular content is a judgement call, so this always
/// reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LineSpacingRule : IContentRule
{
    private const double MinimumLineHeight = 1.5;

    public string RuleId => "line-spacing";

    public string SuccessCriterion => "1.4.8";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<TextStyleFragment>())
        {
            if (fragment.LineHeight is not < MinimumLineHeight)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"Text \"{fragment.SampleText}\" has a line-height of {fragment.LineHeight:F2}, below the AAA-recommended minimum of {MinimumLineHeight:F1}x. Consider increasing line spacing for readability.");
        }
    }
}

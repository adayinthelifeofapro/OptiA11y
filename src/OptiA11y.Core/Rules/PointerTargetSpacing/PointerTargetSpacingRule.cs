using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PointerTargetSpacing;

/// <summary>
/// Flags an interactive target that is both under the 24px WCAG 2.5.8 minimum and closer than
/// 24px to its nearest interactive neighbour, so neither the minimum-size nor the
/// sufficient-offset exception applies. <see cref="PointerTargetSpacingFragment"/> is only ever
/// produced by the rendered-style enrichment slice. Whether the specific spacing is genuinely
/// problematic for this layout is a judgement call, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class PointerTargetSpacingRule : IContentRule
{
    public string RuleId => "pointer-target-spacing";

    public string SuccessCriterion => "2.5.8";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var target in document.Get<PointerTargetSpacingFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                target.Location,
                $"{target.ElementDescription} is smaller than 24x24px and sits closer than 24px to its nearest interactive neighbour. Neither the minimum-size nor sufficient-offset exception applies here - consider enlarging it or increasing spacing.");
        }
    }
}

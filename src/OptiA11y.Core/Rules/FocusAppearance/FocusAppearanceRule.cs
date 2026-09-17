using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FocusAppearance;

/// <summary>
/// Flags a focus indicator estimated to fall below the WCAG 2.4.13 (AAA) appearance thresholds.
/// <see cref="FocusAppearanceFragment"/> is only ever produced by the rendered-style enrichment
/// slice. Estimating true perimeter/contrast area against the AAA formula from computed styles
/// alone is approximate, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FocusAppearanceRule : IContentRule
{
    public string RuleId => "focus-appearance";

    public string SuccessCriterion => "2.4.13";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var appearance in document.Get<FocusAppearanceFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                appearance.Location,
                $"{appearance.ElementDescription}'s focus indicator appears thinner than a 2px outline and has no equivalent box-shadow. Confirm it meets the AAA focus appearance thresholds (a sufficiently thick, high-contrast indicator around the whole control).");
        }
    }
}

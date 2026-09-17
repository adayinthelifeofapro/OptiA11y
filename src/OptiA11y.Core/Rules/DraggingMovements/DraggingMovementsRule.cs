using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DraggingMovements;

/// <summary>
/// Flags an element that exposes drag behaviour with no discoverable single-pointer alternative
/// such as a button (WCAG 2.5.7). <see cref="DraggingMovementsFragment"/> is only ever produced
/// by the rendered-style enrichment slice. Confirming the absence of any alternative elsewhere on
/// the page is not fully verifiable, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class DraggingMovementsRule : IContentRule
{
    public string RuleId => "dragging-movements";

    public string SuccessCriterion => "2.5.7";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<DraggingMovementsFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.NeedsReview,
                element.Location,
                $"{element.ElementDescription} appears to require a drag gesture. Confirm a single-pointer alternative (like up/down buttons) exists for users who can't perform a drag.");
        }
    }
}

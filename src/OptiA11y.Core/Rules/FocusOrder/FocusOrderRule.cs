using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FocusOrder;

/// <summary>
/// Flags a page whose DOM/keyboard tab order visits focusable elements in a different sequence
/// than their visual layout order (WCAG 2.4.3). <see cref="FocusOrderFragment"/> is only ever
/// produced by the rendered-style enrichment slice. Whether a specific divergence is actually
/// disorienting depends on the page's intent (e.g. a deliberately reordered mobile layout), so
/// this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FocusOrderRule : IContentRule
{
    public string RuleId => "focus-order";

    public string SuccessCriterion => "2.4.3";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var order in document.Get<FocusOrderFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.NeedsReview,
                order.Location,
                "The keyboard tab order on this page diverges from its visual layout order. Confirm this is intentional - if not, users tabbing through the page will encounter controls in a confusing sequence.");
        }
    }
}

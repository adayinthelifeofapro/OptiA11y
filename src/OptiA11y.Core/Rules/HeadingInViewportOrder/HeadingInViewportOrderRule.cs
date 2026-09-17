using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.HeadingInViewportOrder;

/// <summary>
/// Flags a page whose heading order in the DOM diverges from the order headings appear visually
/// on screen (WCAG 1.3.2). <see cref="HeadingInViewportOrderFragment"/> is only ever produced by
/// the rendered-style enrichment slice. Whether a given divergence actually confuses
/// assistive-technology users (who navigate by DOM heading order) is a judgement call, so this
/// always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class HeadingInViewportOrderRule : IContentRule
{
    public string RuleId => "heading-in-viewport-order";

    public string SuccessCriterion => "1.3.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var heading in document.Get<HeadingInViewportOrderFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                heading.Location,
                "This page's heading elements appear in a different order in the DOM than they do visually on screen. Screen reader users navigating by heading may encounter them in a sequence that doesn't match what sighted users see.");
        }
    }
}

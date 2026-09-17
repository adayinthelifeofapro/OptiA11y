using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.HoverFocusContent;

/// <summary>
/// Flags an element that reveals additional content on hover/focus which disappears as soon as
/// the pointer moves away or focus is lost, without being hoverable, dismissible, or persistent
/// (WCAG 1.4.13). <see cref="HoverFocusContentFragment"/> is only ever produced by the
/// rendered-style enrichment slice. Whether this specific pattern is genuinely a problem depends
/// on details this heuristic cannot fully see, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class HoverFocusContentRule : IContentRule
{
    public string RuleId => "hover-focus-content";

    public string SuccessCriterion => "1.4.13";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<HoverFocusContentFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                element.Location,
                $"{element.ElementDescription} appears to reveal content on hover/focus that disappears immediately once the pointer moves away. Confirm the revealed content is dismissible, hoverable (doesn't disappear when the pointer moves onto it), and persists until dismissed.");
        }
    }
}

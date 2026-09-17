using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.StatusMessages;

/// <summary>
/// Flags content inserted into the page dynamically into an element lacking any live-region
/// semantics (WCAG 4.1.3). <see cref="StatusMessagesFragment"/> is only ever produced by the
/// rendered-style enrichment slice. Whether the specific content genuinely needs to be announced
/// is a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class StatusMessagesRule : IContentRule
{
    public string RuleId => "status-messages";

    public string SuccessCriterion => "4.1.3";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<StatusMessagesFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                element.Location,
                $"{element.ElementDescription} receives dynamically inserted content but has no aria-live, role=\"status\", or role=\"alert\". If this content conveys a status update, screen reader users may miss it entirely.");
        }
    }
}

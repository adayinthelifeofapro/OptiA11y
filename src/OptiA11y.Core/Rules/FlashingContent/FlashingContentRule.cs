using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FlashingContent;

/// <summary>
/// Flags media whose source URL or surrounding class/data attributes mention flash, strobe, or
/// flicker (e.g. "flash-intro.mp4", class="strobe-effect") - an authoring-time hint that the
/// media may contain rapid flashing content that could trigger seizures (WCAG 2.3.1). This is a
/// weak textual heuristic, not a pixel-level flash-rate measurement (see the rendered-only
/// <c>flash-threshold</c> rule for that), so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FlashingContentRule : IContentRule
{
    public string RuleId => "flashing-content";

    public string SuccessCriterion => "2.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!media.HasFlashIndicator)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                media.Location,
                $"This {media.MediaKind}'s source or surrounding markup mentions flash/strobe/flicker. Confirm it does not exceed the general flash threshold (no more than 3 flashes per second), or provide a warning.");
        }
    }
}

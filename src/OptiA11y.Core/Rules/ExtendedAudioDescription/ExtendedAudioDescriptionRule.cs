using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ExtendedAudioDescription;

/// <summary>
/// Flags description-heavy video (one that already has a standard audio-description track) that
/// has no extended-description track, which pauses the video to fit in longer descriptions
/// (WCAG 1.2.7, AAA). Whether the standard track's pauses are actually long enough is a judgement
/// call this rule cannot make from markup alone, so it always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class ExtendedAudioDescriptionRule : IContentRule
{
    public string RuleId => "extended-audio-description";

    public string SuccessCriterion => "1.2.7";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!string.Equals(media.MediaKind, "video", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!media.HasDescriptionTrack || media.HasExtendedDescriptionTrack)
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
                "This video has a standard audio-description track but no extended-description track. If the standard track's pauses aren't long enough to convey the necessary visual information, provide an extended audio description (WCAG 1.2.7, AAA).");
        }
    }
}

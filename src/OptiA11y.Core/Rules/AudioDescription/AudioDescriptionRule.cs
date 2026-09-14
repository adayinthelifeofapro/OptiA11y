using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AudioDescription;

/// <summary>
/// Flags video with no descriptions track and no nearby mention of audio description, so blind
/// or low-vision users may be missing visual information conveyed only on screen (e.g. actions,
/// scene changes) that the dialogue alone doesn't cover. Detecting a &lt;track kind="descriptions"&gt;
/// is deterministic, but "does this video actually need audio description" is a judgement about
/// the content that this rule cannot make, so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class AudioDescriptionRule : IContentRule
{
    public string RuleId => "audio-description";

    public string SuccessCriterion => "1.2.5";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!string.Equals(media.MediaKind, "video", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (media.HasDescriptionTrack)
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
                "This video has no audio-description track. If the visuals convey information that isn't already in the dialogue (actions, on-screen text, scene changes), add an audio-described version or a descriptions track.");
        }
    }
}

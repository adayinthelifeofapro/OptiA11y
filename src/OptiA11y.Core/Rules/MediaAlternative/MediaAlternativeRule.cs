using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.MediaAlternative;

/// <summary>
/// Flags video that has neither an audio-description track nor a full text alternative
/// (transcript) - WCAG 1.2.3 permits either. Detecting the absence of both mechanisms is
/// deterministic, but a nearby "transcript" link is only a heuristic signal that a full text
/// alternative exists, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class MediaAlternativeRule : IContentRule
{
    public string RuleId => "media-alternative";

    public string SuccessCriterion => "1.2.3";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!string.Equals(media.MediaKind, "video", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (media.HasDescriptionTrack || media.HasCaptionsOrTranscript)
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
                "This video has neither an audio-description track nor a transcript/text alternative. Provide at least one so users who can't perceive the video's visuals still get the information it conveys (WCAG 1.2.3).");
        }
    }
}

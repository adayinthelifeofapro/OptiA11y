using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.MediaCaptions;

/// <summary>
/// Evaluates video/audio elements for captions or a transcript. Whether a media element has no
/// caption track and no nearby transcript-like link at all is treated as a deterministic
/// structural fact for video (which conveys visual information a transcript alone cannot fully
/// replace), so that case is <see cref="Confidence.Fail"/>. Audio-only elements can be
/// adequately served by a transcript alone, and detecting "adequately captioned" more precisely
/// than presence/absence of a track is a judgement call, so audio findings are always
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class MediaCaptionsRule : IContentRule
{
    public string RuleId => "media-captions";

    public string SuccessCriterion => "1.2.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (media.HasCaptionsOrTranscript)
            {
                continue;
            }

            if (string.Equals(media.MediaKind, "video", StringComparison.OrdinalIgnoreCase))
            {
                yield return Fail(media, "This video has no captions track and no nearby transcript. Add captions so deaf or hard-of-hearing users can access the content.");
            }
            else
            {
                yield return NeedsReview(media, "This audio has no captions track and no nearby transcript link was detected. Confirm a transcript exists, or add one.");
            }
        }
    }

    private Finding Fail(MediaFragment media, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Critical,
        Confidence.Fail,
        media.Location,
        message);

    private Finding NeedsReview(MediaFragment media, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.NeedsReview,
        media.Location,
        message);
}

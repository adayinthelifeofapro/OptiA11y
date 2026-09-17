using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.MediaTranscriptQuality;

/// <summary>
/// Flags a "transcript" link whose href points at the media file itself (not a text document) or
/// whose link text is too weak to be trusted as a genuine transcript (e.g. just "here" or
/// "transcript"). A link pointing at an audio/video file cannot be a text transcript - that part
/// is a deterministic structural fact - but judging whether weaker link text still leads to a
/// real transcript is a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class MediaTranscriptQualityRule : IContentRule
{
    private static readonly string[] MediaExtensions =
    {
        ".mp4", ".mp3", ".wav", ".m4a", ".mov", ".webm", ".ogg", ".avi"
    };

    private static readonly string[] WeakLinkTexts =
    {
        "here", "click here", "transcript", "link", "this link"
    };

    public string RuleId => "media-transcript-quality";

    public string SuccessCriterion => "1.2.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (media.TranscriptHref is null)
            {
                continue;
            }

            var pointsAtMediaFile = MediaExtensions.Any(ext => media.TranscriptHref.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
            var hasWeakText = media.TranscriptLinkText is null
                || WeakLinkTexts.Contains(media.TranscriptLinkText.Trim(), StringComparer.OrdinalIgnoreCase);

            if (!pointsAtMediaFile && !hasWeakText)
            {
                continue;
            }

            var message = pointsAtMediaFile
                ? "The nearby \"transcript\" link points at the media file itself, not a text alternative. Link to an actual text transcript."
                : $"The nearby transcript link's text (\"{media.TranscriptLinkText}\") is too generic to confirm it leads to a real transcript. Verify it does, and consider more descriptive link text.";

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                media.Location,
                message);
        }
    }
}

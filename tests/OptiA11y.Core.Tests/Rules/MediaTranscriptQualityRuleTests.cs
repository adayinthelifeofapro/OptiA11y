using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.MediaTranscriptQuality;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class MediaTranscriptQualityRuleTests
{
    private readonly MediaTranscriptQualityRule _rule = new();

    [Fact]
    public void NoTranscriptLink_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void TranscriptLinkPointsAtMediaFile_IsNeedsReview()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            TranscriptHref: "video.mp4",
            TranscriptLinkText: "Read the transcript");
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void TranscriptLinkHasWeakText_IsNeedsReview()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            TranscriptHref: "transcript.html",
            TranscriptLinkText: "here");
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
    }

    [Fact]
    public void TranscriptLinkIsGoodQuality_ProducesNoFindings()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            TranscriptHref: "transcript.html",
            TranscriptLinkText: "Read the full transcript of this video");
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

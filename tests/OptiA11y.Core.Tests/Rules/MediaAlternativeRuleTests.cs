using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.MediaAlternative;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class MediaAlternativeRuleTests
{
    private readonly MediaAlternativeRule _rule = new();

    [Fact]
    public void VideoWithNeitherAlternative_IsNeedsReview()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            HasCaptionsOrTranscript: false,
            HasDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void VideoWithDescriptionTrack_ProducesNoFindings()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            HasCaptionsOrTranscript: false,
            HasDescriptionTrack: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VideoWithTranscript_ProducesNoFindings()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            HasCaptionsOrTranscript: true,
            HasDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

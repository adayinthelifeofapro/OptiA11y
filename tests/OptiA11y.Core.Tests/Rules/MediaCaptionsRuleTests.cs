using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.MediaCaptions;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class MediaCaptionsRuleTests
{
    private readonly MediaCaptionsRule _rule = new();

    [Fact]
    public void NoMedia_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VideoWithoutCaptionsOrTranscript_IsFail()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void AudioWithoutCaptionsOrTranscript_IsNeedsReview()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "audio.mp3", "audio", HasCaptionsOrTranscript: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void MediaWithCaptionsOrTranscript_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

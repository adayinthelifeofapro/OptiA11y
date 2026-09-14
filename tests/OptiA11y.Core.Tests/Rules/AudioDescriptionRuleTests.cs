using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AudioDescription;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AudioDescriptionRuleTests
{
    private readonly AudioDescriptionRule _rule = new();

    [Fact]
    public void NoMedia_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VideoWithoutDescriptionTrack_IsNeedsReview()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, HasDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void VideoWithDescriptionTrack_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, HasDescriptionTrack: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void Audio_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "audio.mp3", "audio", HasCaptionsOrTranscript: true, HasDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

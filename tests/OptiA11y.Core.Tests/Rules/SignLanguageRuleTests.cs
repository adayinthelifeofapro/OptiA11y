using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SignLanguage;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SignLanguageRuleTests
{
    private readonly SignLanguageRule _rule = new();

    [Fact]
    public void VideoWithoutSignLanguageTrack_IsNeedsReview()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", true, HasSignLanguageTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void VideoWithSignLanguageTrack_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", true, HasSignLanguageTrack: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void Audio_IsIgnored()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "audio.mp3", "audio", true, HasSignLanguageTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

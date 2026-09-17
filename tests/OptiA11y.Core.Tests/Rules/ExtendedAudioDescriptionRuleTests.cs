using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ExtendedAudioDescription;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ExtendedAudioDescriptionRuleTests
{
    private readonly ExtendedAudioDescriptionRule _rule = new();

    [Fact]
    public void VideoWithStandardDescriptionOnly_IsNeedsReview()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            HasDescriptionTrack: true,
            HasExtendedDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void VideoWithExtendedDescription_ProducesNoFindings()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            HasDescriptionTrack: true,
            HasExtendedDescriptionTrack: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VideoWithNoDescriptionAtAll_ProducesNoFindings()
    {
        var media = new MediaFragment(
            TestLocations.OnMainBody(),
            "video.mp4",
            "video",
            true,
            HasDescriptionTrack: false,
            HasExtendedDescriptionTrack: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

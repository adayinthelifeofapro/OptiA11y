using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FlashingContent;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FlashingContentRuleTests
{
    private readonly FlashingContentRule _rule = new();

    [Fact]
    public void MediaWithFlashIndicator_IsNeedsReview()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "flash-intro.mp4", "video", true, HasFlashIndicator: true);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void MediaWithoutFlashIndicator_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", true, HasFlashIndicator: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

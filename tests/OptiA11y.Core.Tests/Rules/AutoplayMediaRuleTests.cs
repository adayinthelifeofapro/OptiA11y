using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AutoplayMedia;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AutoplayMediaRuleTests
{
    private readonly AutoplayMediaRule _rule = new();

    [Fact]
    public void NoMedia_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void AutoplayingUnmutedWithoutControls_IsFail()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, Autoplay: true, Muted: false, HasControls: false);
        var document = new AuditDocument("content-1", new[] { media });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void AutoplayingMuted_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, Autoplay: true, Muted: true, HasControls: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void AutoplayingWithControls_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, Autoplay: true, Muted: false, HasControls: true);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NotAutoplaying_ProducesNoFindings()
    {
        var media = new MediaFragment(TestLocations.OnMainBody(), "video.mp4", "video", HasCaptionsOrTranscript: true, Autoplay: false);
        var document = new AuditDocument("content-1", new[] { media });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LiveRegionMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LiveRegionMisuseRuleTests
{
    private readonly LiveRegionMisuseRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void InvalidPolitenessValue_IsNeedsReview()
    {
        var fragment = new LiveRegionFragment(TestLocations.OnMainBody(), "div", null, "eventually", HasInvalidPolitenessValue: true, TextLength: 10);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LargeStaticLookingContent_IsNeedsReview()
    {
        var fragment = new LiveRegionFragment(TestLocations.OnMainBody(), "div", "status", "polite", HasInvalidPolitenessValue: false, TextLength: 500);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ValidShortLiveRegion_ProducesNoFindings()
    {
        var fragment = new LiveRegionFragment(TestLocations.OnMainBody(), "div", "status", "polite", HasInvalidPolitenessValue: false, TextLength: 20);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

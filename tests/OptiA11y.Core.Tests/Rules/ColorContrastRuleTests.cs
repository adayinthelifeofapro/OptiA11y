using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ColorContrast;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ColorContrastRuleTests
{
    private readonly ColorContrastRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void LowContrastNormalText_IsFail()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#777", "#666", 1.2, IsLargeText: false, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void SufficientContrast_ProducesNoFindings()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#000", "#fff", 21.0, IsLargeText: false, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void LowContrastOverImageBackground_IsNeedsReview()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#777", "#666", 1.2, IsLargeText: false, "Sample text", BackgroundIsImage: true);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

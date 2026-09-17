using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ContrastEnhanced;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ContrastEnhancedRuleTests
{
    private readonly ContrastEnhancedRule _rule = new();

    [Fact]
    public void NormalTextBelowSevenToOne_IsFail()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#767676", "#ffffff", 5.0, false, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NormalTextAtOrAboveSevenToOne_ProducesNoFindings()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#000000", "#ffffff", 21.0, false, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void ImageBackground_IsNeedsReview()
    {
        var fragment = new ColorContrastFragment(TestLocations.OnMainBody(), "#ffffff", "#000000", 2.0, false, "Sample text", BackgroundIsImage: true);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

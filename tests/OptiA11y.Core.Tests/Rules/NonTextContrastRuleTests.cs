using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.NonTextContrast;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class NonTextContrastRuleTests
{
    private readonly NonTextContrastRule _rule = new();

    [Fact]
    public void LowContrastAgainstExplicitBackground_IsFail()
    {
        var fragment = new NonTextContrastFragment(TestLocations.OnMainBody(), "#cccccc", "#ffffff", 1.6, "<div>.icon");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LowContrastAgainstNonDefaultBackground_IsFail()
    {
        var fragment = new NonTextContrastFragment(TestLocations.OnMainBody(), "#cccccc", "#dddddd", 1.2, "<div>.icon");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void SufficientContrast_ProducesNoFindings()
    {
        var fragment = new NonTextContrastFragment(TestLocations.OnMainBody(), "#000000", "#ffffff", 21.0, "<div>.icon");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

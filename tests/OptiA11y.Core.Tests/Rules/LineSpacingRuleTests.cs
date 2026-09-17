using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LineSpacing;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LineSpacingRuleTests
{
    private readonly LineSpacingRule _rule = new();

    [Fact]
    public void TightLineHeight_IsNeedsReview()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", LineHeight: 1.1);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SufficientLineHeight_ProducesNoFindings()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", LineHeight: 1.6);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoLineHeight_ProducesNoFindings()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

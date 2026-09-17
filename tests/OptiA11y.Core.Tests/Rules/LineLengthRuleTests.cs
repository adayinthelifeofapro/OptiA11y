using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LineLength;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LineLengthRuleTests
{
    private readonly LineLengthRule _rule = new();

    [Fact]
    public void WideBlock_IsNeedsReview()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", WidthPx: 1200);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void NarrowBlock_ProducesNoFindings()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", WidthPx: 600);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoWidth_ProducesNoFindings()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

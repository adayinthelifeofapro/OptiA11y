using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.WhitespaceFormatting;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class WhitespaceFormattingRuleTests
{
    private readonly WhitespaceFormattingRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void RepeatedNonBreakingSpaces_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Item\u00A0\u00A0\u00A0Price", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void RepeatedRegularSpaces_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Item     Price", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void NormalSpacing_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Item Price", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

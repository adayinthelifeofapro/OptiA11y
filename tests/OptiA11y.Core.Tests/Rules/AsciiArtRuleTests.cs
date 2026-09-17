using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AsciiArt;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AsciiArtRuleTests
{
    private readonly AsciiArtRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void PunctuationDivider_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "----------------", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void PlainText_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Regular sentence about our products.", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.BackgroundImageText;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class BackgroundImageTextRuleTests
{
    private readonly BackgroundImageTextRule _rule = new();

    [Fact]
    public void TextWithBackgroundImage_IsNeedsReview()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", HasBackgroundImage: true);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void TextWithoutBackgroundImage_ProducesNoFindings()
    {
        var fragment = new TextStyleFragment(TestLocations.OnMainBody(), null, null, "Sample text", HasBackgroundImage: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

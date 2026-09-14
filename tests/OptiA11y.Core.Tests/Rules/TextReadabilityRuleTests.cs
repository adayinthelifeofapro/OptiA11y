using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.TextReadability;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class TextReadabilityRuleTests
{
    private readonly TextReadabilityRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void JustifiedText_IsFail()
    {
        var style = new TextStyleFragment(TestLocations.OnMainBody(), "justify", 16, "Sample text");
        var document = new AuditDocument("content-1", new[] { style });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void TinyFont_IsFail()
    {
        var style = new TextStyleFragment(TestLocations.OnMainBody(), "left", 9, "Sample text");
        var document = new AuditDocument("content-1", new[] { style });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void LongAllCapsRun_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "PLEASE READ THIS IMPORTANT NOTICE before continuing.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ShortAcronym_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "See the WCAG guidelines for details.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LanguageOfParts;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LanguageOfPartsRuleTests
{
    private readonly LanguageOfPartsRule _rule = new();

    [Fact]
    public void NoText_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NonLatinScriptWithoutLangOverride_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Please say добро пожаловать to our guests.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void NonLatinScriptWithLangOverride_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "добро пожаловать", LanguageCode: "ru");
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void PlainEnglishText_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "This is a plain English sentence.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }
}

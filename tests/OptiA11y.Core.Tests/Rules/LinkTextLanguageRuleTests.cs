using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LinkTextLanguage;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LinkTextLanguageRuleTests
{
    private readonly LinkTextLanguageRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DifferentScriptWithNoLang_IsNeedsReview()
    {
        var fragment = new LinkFragment(TestLocations.OnMainBody(), "/page", "Привет мир", IsDocumentLink: false, LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DifferentScriptWithLang_ProducesNoFindings()
    {
        var fragment = new LinkFragment(TestLocations.OnMainBody(), "/page", "Привет мир", IsDocumentLink: false, LanguageCode: "ru");
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void EnglishLinkText_ProducesNoFindings()
    {
        var fragment = new LinkFragment(TestLocations.OnMainBody(), "/page", "Read more", IsDocumentLink: false, LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

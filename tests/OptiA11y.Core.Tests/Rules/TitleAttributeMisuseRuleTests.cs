using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.TitleAttributeMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class TitleAttributeMisuseRuleTests
{
    private readonly TitleAttributeMisuseRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void LinkTitleDuplicatesVisibleText_IsNeedsReview()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/about", "About us", IsDocumentLink: false, HasAccessibleName: true, TitleAttribute: "About us");
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LinkWithNoVisibleTextReliesOnTitle_IsNeedsReview()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/about", "", IsDocumentLink: false, HasAccessibleName: true, TitleAttribute: "About us");
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LinkTitleAddsExtraInformation_ProducesNoFindings()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/about", "About us", IsDocumentLink: false, HasAccessibleName: true, TitleAttribute: "Learn about our history and team");
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void ButtonWithNoOtherAccessibleNameReliesOnTitle_IsNeedsReview()
    {
        var button = new ButtonFragment(TestLocations.OnMainBody(), "button", HasAccessibleName: false, TitleAttribute: "Close");
        var document = new AuditDocument("content-1", new[] { button });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

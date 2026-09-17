using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LinkDistinguishability;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LinkDistinguishabilityRuleTests
{
    private readonly LinkDistinguishabilityRule _rule = new();

    [Fact]
    public void LinkWithColorAndNoUnderline_IsNeedsReview()
    {
        var link = new LinkFragment(
            TestLocations.OnMainBody(),
            "/page",
            "Learn more",
            IsDocumentLink: false,
            HasExplicitColorStyle: true,
            RemovesUnderline: true);
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LinkWithColorButUnderlineKept_ProducesNoFindings()
    {
        var link = new LinkFragment(
            TestLocations.OnMainBody(),
            "/page",
            "Learn more",
            IsDocumentLink: false,
            HasExplicitColorStyle: true,
            RemovesUnderline: false);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void LinkWithNoExplicitColor_ProducesNoFindings()
    {
        var link = new LinkFragment(
            TestLocations.OnMainBody(),
            "/page",
            "Learn more",
            IsDocumentLink: false,
            HasExplicitColorStyle: false,
            RemovesUnderline: true);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }
}

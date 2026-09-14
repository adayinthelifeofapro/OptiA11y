using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LinkName;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LinkNameRuleTests
{
    private readonly LinkNameRule _rule = new();

    [Fact]
    public void NoLinks_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void LinkWithNoAccessibleName_IsFail()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/page", "", IsDocumentLink: false, HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void LinkWithAccessibleName_ProducesNoFindings()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/page", "About us", IsDocumentLink: false, HasAccessibleName: true);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.IframeTitle;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class IframeTitleRuleTests
{
    private readonly IframeTitleRule _rule = new();

    [Fact]
    public void NoIframes_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void IframeWithoutAccessibleName_IsFail()
    {
        var iframe = new IframeFragment(TestLocations.OnMainBody(), "https://example.com/embed", HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { iframe });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void IframeWithAccessibleName_ProducesNoFindings()
    {
        var iframe = new IframeFragment(TestLocations.OnMainBody(), "https://example.com/embed", HasAccessibleName: true);
        var document = new AuditDocument("content-1", new[] { iframe });

        Assert.Empty(_rule.Evaluate(document));
    }
}

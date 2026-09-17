using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.MetaRefresh;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class MetaRefreshRuleTests
{
    private readonly MetaRefreshRule _rule = new();

    [Fact]
    public void MetaRefreshTag_IsFail()
    {
        var metaRefresh = new MetaRefreshFragment(TestLocations.OnMainBody(), "5;url=https://example.com");
        var document = new AuditDocument("content-1", new[] { metaRefresh });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NoMetaRefresh_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }
}

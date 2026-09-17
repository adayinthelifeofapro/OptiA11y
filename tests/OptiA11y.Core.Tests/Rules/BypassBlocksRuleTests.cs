using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.BypassBlocks;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class BypassBlocksRuleTests
{
    private readonly BypassBlocksRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoBypassMechanism_IsFail()
    {
        var fragment = new BypassBlocksFragment(TestLocations.OnMainBody());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.KeyboardTrap;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class KeyboardTrapRuleTests
{
    private readonly KeyboardTrapRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void KeyboardTrap_IsFail()
    {
        var fragment = new KeyboardTrapFragment(TestLocations.OnMainBody());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }
}

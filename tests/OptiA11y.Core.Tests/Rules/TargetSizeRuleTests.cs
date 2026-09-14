using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.TargetSize;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class TargetSizeRuleTests
{
    private readonly TargetSizeRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void UndersizedTarget_IsFail()
    {
        var target = new TargetSizeFragment(TestLocations.OnMainBody(), "<button> \"Close\"", 18, 18);
        var document = new AuditDocument("content-1", new[] { target });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void SufficientlySizedTarget_ProducesNoFindings()
    {
        var target = new TargetSizeFragment(TestLocations.OnMainBody(), "<button> \"Close\"", 44, 44);
        var document = new AuditDocument("content-1", new[] { target });

        Assert.Empty(_rule.Evaluate(document));
    }
}

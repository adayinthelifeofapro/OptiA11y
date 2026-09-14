using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.NestedInteractive;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class NestedInteractiveRuleTests
{
    private readonly NestedInteractiveRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NestedInteractiveElement_IsFail()
    {
        var fragment = new InteractiveAttributesFragment(
            TestLocations.OnMainBody(), "button", TabIndex: null, AriaHidden: false, Role: null, AccessKey: null,
            IsNativelyInteractive: true, IsNestedInInteractiveAncestor: true);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NonNestedInteractiveElement_ProducesNoFindings()
    {
        var fragment = new InteractiveAttributesFragment(
            TestLocations.OnMainBody(), "button", TabIndex: null, AriaHidden: false, Role: null, AccessKey: null,
            IsNativelyInteractive: true, IsNestedInInteractiveAncestor: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

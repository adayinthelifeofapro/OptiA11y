using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.InvalidAria;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class InvalidAriaRuleTests
{
    private readonly InvalidAriaRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void UnknownRole_IsFail()
    {
        var fragment = new AriaAttributesFragment(TestLocations.OnMainBody(), "div", "banenr", RoleIsUnknown: true, UnknownAriaAttributeNames: Array.Empty<string>());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void UnknownAriaAttribute_IsFail()
    {
        var fragment = new AriaAttributesFragment(TestLocations.OnMainBody(), "div", Role: null, RoleIsUnknown: false, UnknownAriaAttributeNames: new[] { "aria-lable" });
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void ValidRoleAndAttributes_ProducesNoFindings()
    {
        var fragment = new AriaAttributesFragment(TestLocations.OnMainBody(), "div", "banner", RoleIsUnknown: false, UnknownAriaAttributeNames: Array.Empty<string>());
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

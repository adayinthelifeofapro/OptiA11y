using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AriaHiddenFocusable;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AriaHiddenFocusableRuleTests
{
    private readonly AriaHiddenFocusableRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void AriaHiddenWithFocusableDescendant_IsFail()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "div", null,
            MissingRequiredOwnedElementDescription: null,
            MissingRequiredAttributes: Array.Empty<string>(),
            DisallowedWidgetStateAttributes: Array.Empty<string>(),
            IsAriaHiddenWithFocusableDescendant: true,
            IsFocusable: false,
            HasGlobalAriaAttribute: false,
            IsRedundantRole: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NotAriaHiddenWithFocusable_ProducesNoFindings()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "div", null,
            MissingRequiredOwnedElementDescription: null,
            MissingRequiredAttributes: Array.Empty<string>(),
            DisallowedWidgetStateAttributes: Array.Empty<string>(),
            IsAriaHiddenWithFocusableDescendant: false,
            IsFocusable: false,
            HasGlobalAriaAttribute: false,
            IsRedundantRole: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

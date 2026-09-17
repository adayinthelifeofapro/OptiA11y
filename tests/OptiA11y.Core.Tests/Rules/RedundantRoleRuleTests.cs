using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.RedundantRole;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class RedundantRoleRuleTests
{
    private readonly RedundantRoleRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void RedundantRole_IsNeedsReview()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "button", "button",
            MissingRequiredOwnedElementDescription: null,
            MissingRequiredAttributes: Array.Empty<string>(),
            DisallowedWidgetStateAttributes: Array.Empty<string>(),
            IsAriaHiddenWithFocusableDescendant: false,
            IsFocusable: true,
            HasGlobalAriaAttribute: false,
            IsRedundantRole: true);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void NonRedundantRole_ProducesNoFindings()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "div", "button",
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

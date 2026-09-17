using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.PresentationRoleConflict;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class PresentationRoleConflictRuleTests
{
    private readonly PresentationRoleConflictRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void PresentationRoleOnFocusableElement_IsFail()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "a", "presentation",
            MissingRequiredOwnedElementDescription: null,
            MissingRequiredAttributes: Array.Empty<string>(),
            DisallowedWidgetStateAttributes: Array.Empty<string>(),
            IsAriaHiddenWithFocusableDescendant: false,
            IsFocusable: true,
            HasGlobalAriaAttribute: false,
            IsRedundantRole: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NoneRoleWithGlobalAriaAttribute_IsFail()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "div", "none",
            MissingRequiredOwnedElementDescription: null,
            MissingRequiredAttributes: Array.Empty<string>(),
            DisallowedWidgetStateAttributes: Array.Empty<string>(),
            IsAriaHiddenWithFocusableDescendant: false,
            IsFocusable: false,
            HasGlobalAriaAttribute: true,
            IsRedundantRole: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void PresentationRoleOnNonFocusableNonAriaElement_ProducesNoFindings()
    {
        var fragment = new AriaSemanticsFragment(
            TestLocations.OnMainBody(), "div", "presentation",
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

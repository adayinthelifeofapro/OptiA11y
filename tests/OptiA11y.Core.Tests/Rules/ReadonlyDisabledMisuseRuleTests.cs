using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ReadonlyDisabledMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ReadonlyDisabledMisuseRuleTests
{
    private readonly ReadonlyDisabledMisuseRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DisabledAndRequired_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            IsDisabled: true, IsRequired: true);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void AriaDisabledAndVisualRequiredIndicator_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            AriaDisabled: true, HasVisualRequiredIndicator: true);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DisabledNotRequired_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            IsDisabled: true, IsRequired: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void RequiredNotDisabled_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            IsDisabled: false, IsRequired: true);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

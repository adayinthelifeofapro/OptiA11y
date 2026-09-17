using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.RequiredFieldIndication;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class RequiredFieldIndicationRuleTests
{
    private readonly RequiredFieldIndicationRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VisualIndicatorWithoutRequiredAttribute_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            HasVisualRequiredIndicator: true, IsRequired: false);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void VisualIndicatorWithRequiredAttribute_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            HasVisualRequiredIndicator: true, IsRequired: true);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoVisualIndicator_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            HasVisualRequiredIndicator: false, IsRequired: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

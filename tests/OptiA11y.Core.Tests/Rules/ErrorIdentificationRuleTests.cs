using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ErrorIdentification;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ErrorIdentificationRuleTests
{
    private readonly ErrorIdentificationRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void AriaInvalidWithoutDescribedBy_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            AriaInvalid: true, HasAriaDescribedBy: false);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void AriaInvalidWithDescribedBy_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            AriaInvalid: true, HasAriaDescribedBy: true);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NotAriaInvalid_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            AriaInvalid: false, HasAriaDescribedBy: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

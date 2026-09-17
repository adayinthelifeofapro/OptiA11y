using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FormInstructions;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FormInstructionsRuleTests
{
    private readonly FormInstructionsRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void FormatConstraintWithoutDescribedBy_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "email", HasAccessibleName: true,
            HasPatternOrFormatConstraint: true, HasAriaDescribedBy: false);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void FormatConstraintWithDescribedBy_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "email", HasAccessibleName: true,
            HasPatternOrFormatConstraint: true, HasAriaDescribedBy: true);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoFormatConstraint_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            HasPatternOrFormatConstraint: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

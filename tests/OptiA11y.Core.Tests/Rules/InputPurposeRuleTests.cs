using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.InputPurpose;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class InputPurposeRuleTests
{
    private readonly InputPurposeRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void InferredEmailFieldWithoutAutocomplete_IsNeedsReview()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "email", HasAccessibleName: true, AutocompleteToken: null, InferredPurposeCategory: "email");
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void InferredEmailFieldWithAutocomplete_ProducesNoFindings()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "email", HasAccessibleName: true, AutocompleteToken: "email", InferredPurposeCategory: "email");
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoInferredPurpose_ProducesNoFindings()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true, AutocompleteToken: null, InferredPurposeCategory: null);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

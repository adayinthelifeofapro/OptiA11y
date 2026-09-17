using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LabelQuality;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LabelQualityRuleTests
{
    private readonly LabelQualityRule _rule = new();

    [Theory]
    [InlineData("Field 1")]
    [InlineData("Text")]
    [InlineData("Input")]
    [InlineData("Label 2")]
    public void GenericLabel_IsNeedsReview(string label)
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true, LabelText: label);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DescriptiveLabel_ProducesNoFinding()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "email", HasAccessibleName: true, LabelText: "Email address");
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void NoLabel_ProducesNoFinding()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "email", HasAccessibleName: false, LabelText: null);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

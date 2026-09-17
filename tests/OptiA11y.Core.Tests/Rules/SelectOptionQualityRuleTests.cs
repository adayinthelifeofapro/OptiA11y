using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SelectOptionQuality;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SelectOptionQualityRuleTests
{
    private readonly SelectOptionQualityRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SelectWithPlaceholderFirstOption_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "select", null, HasAccessibleName: true,
            FirstOptionText: "Please select");
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SelectWithEmptyOptionText_IsNeedsReview()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "select", null, HasAccessibleName: true,
            FirstOptionText: "Red", HasEmptyOptionText: true);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SelectWithGoodOptions_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "select", null, HasAccessibleName: true,
            FirstOptionText: "Red", HasEmptyOptionText: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NonSelectControl_ProducesNoFindings()
    {
        var field = new FormFieldFragment(
            TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true,
            FirstOptionText: "Please select");
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

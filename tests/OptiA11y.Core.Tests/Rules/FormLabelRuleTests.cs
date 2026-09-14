using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FormLabels;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FormLabelRuleTests
{
    private readonly FormLabelRule _rule = new();

    [Fact]
    public void NoFields_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void UnlabelledTextInput_IsFail()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "text", HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { field });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void LabelledField_ProducesNoFindings()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "text", HasAccessibleName: true);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void HiddenInput_IsExempt()
    {
        var field = new FormFieldFragment(TestLocations.OnMainBody(), "input", "hidden", HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { field });

        Assert.Empty(_rule.Evaluate(document));
    }
}

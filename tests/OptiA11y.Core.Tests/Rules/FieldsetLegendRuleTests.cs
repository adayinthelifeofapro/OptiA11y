using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FieldsetLegend;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FieldsetLegendRuleTests
{
    private readonly FieldsetLegendRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void FieldsetWithoutLegend_IsFail()
    {
        var fieldset = new FieldsetFragment(TestLocations.OnMainBody(), HasLegend: false);
        var document = new AuditDocument("content-1", new[] { fieldset });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void FieldsetWithLegend_ProducesNoFindings()
    {
        var fieldset = new FieldsetFragment(TestLocations.OnMainBody(), HasLegend: true);
        var document = new AuditDocument("content-1", new[] { fieldset });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void OrphanRadioGroup_IsFail()
    {
        var radioGroup = new RadioGroupFragment(TestLocations.OnMainBody(), "shipping-method", 3);
        var document = new AuditDocument("content-1", new[] { radioGroup });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }
}

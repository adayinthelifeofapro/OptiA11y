using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SvgAccessibleName;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SvgAccessibleNameRuleTests
{
    private readonly SvgAccessibleNameRule _rule = new();

    [Fact]
    public void NoElements_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SvgWithoutAccessibleNameOrAriaHidden_IsFail()
    {
        var svg = new NonTextElementFragment(TestLocations.OnMainBody(), "svg", HasAccessibleName: false, IsAriaHidden: false);
        var document = new AuditDocument("content-1", new[] { svg });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void SvgMarkedAriaHidden_ProducesNoFindings()
    {
        var svg = new NonTextElementFragment(TestLocations.OnMainBody(), "svg", HasAccessibleName: false, IsAriaHidden: true);
        var document = new AuditDocument("content-1", new[] { svg });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SvgWithAccessibleName_ProducesNoFindings()
    {
        var svg = new NonTextElementFragment(TestLocations.OnMainBody(), "svg", HasAccessibleName: true, IsAriaHidden: false);
        var document = new AuditDocument("content-1", new[] { svg });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ButtonName;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ButtonNameRuleTests
{
    private readonly ButtonNameRule _rule = new();

    [Fact]
    public void NoButtons_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void ButtonWithoutAccessibleName_IsFail()
    {
        var button = new ButtonFragment(TestLocations.OnMainBody(), "button", HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { button });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void ButtonWithAccessibleName_ProducesNoFindings()
    {
        var button = new ButtonFragment(TestLocations.OnMainBody(), "button", HasAccessibleName: true);
        var document = new AuditDocument("content-1", new[] { button });

        Assert.Empty(_rule.Evaluate(document));
    }
}

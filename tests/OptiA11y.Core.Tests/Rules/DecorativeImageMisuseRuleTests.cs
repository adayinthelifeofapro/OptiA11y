using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.DecorativeImageMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class DecorativeImageMisuseRuleTests
{
    private readonly DecorativeImageMisuseRule _rule = new();

    [Fact]
    public void EmptyAltWithCaption_IsNeedsReview()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "", Caption: "Quarterly revenue by region");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void EmptyAltWithNoCaption_ProducesNoFinding()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "", Caption: null);
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void DescriptiveAltWithCaption_ProducesNoFinding()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "Bar chart of revenue", Caption: "Quarterly revenue by region");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

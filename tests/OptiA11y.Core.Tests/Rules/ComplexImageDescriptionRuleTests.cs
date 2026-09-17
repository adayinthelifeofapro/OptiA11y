using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ComplexImageDescription;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ComplexImageDescriptionRuleTests
{
    private readonly ComplexImageDescriptionRule _rule = new();

    [Theory]
    [InlineData("sales-chart.png", "Sales chart")]
    [InlineData("q3-graph.jpg", "Growth graph")]
    [InlineData("process.png", "Process flowchart")]
    public void ChartLikeImageWithShortAlt_IsNeedsReview(string src, string alt)
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), src, alt, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ChartWithLongDescriptiveAlt_ProducesNoFinding()
    {
        var longAlt = "Bar chart showing quarterly revenue growth from $1.2M in Q1 to $2.4M in Q4, "
            + "with the steepest increase occurring between Q2 and Q3 due to the product launch.";
        var image = new ImageFragment(TestLocations.OnMainBody(), "sales-chart.png", longAlt, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void NonChartImage_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "team-photo.jpg", "The team at the conference", IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void MissingAltText_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "sales-chart.png", AltText: null, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

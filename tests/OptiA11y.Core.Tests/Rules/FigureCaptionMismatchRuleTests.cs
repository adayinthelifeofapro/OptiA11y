using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FigureCaptionMismatch;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FigureCaptionMismatchRuleTests
{
    private readonly FigureCaptionMismatchRule _rule = new();

    [Fact]
    public void IdenticalAltAndCaption_IsNeedsReview()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "Team at the summit", Caption: "Team at the summit");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void CaseInsensitiveMatch_IsNeedsReview()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "TEAM AT THE SUMMIT", Caption: "team at the summit");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
    }

    [Fact]
    public void DifferentAltAndCaption_ProducesNoFinding()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: "The team celebrating at the summit of Mount Rainier", Caption: "Summer 2024 climb");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void MissingAltOrCaption_ProducesNoFinding()
    {
        var figure = new FigureFragment(TestLocations.OnMainBody(), AltText: null, Caption: "Summer 2024 climb");
        var document = new AuditDocument("content-1", new[] { figure });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

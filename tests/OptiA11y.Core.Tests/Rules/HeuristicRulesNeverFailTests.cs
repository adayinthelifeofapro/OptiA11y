using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.LinkPurpose;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

/// <summary>
/// Enforces the honesty mechanism at the centre of OptiA11y: any rule whose logic is a
/// heuristic judgement (not a deterministic structural fact) must never report
/// <see cref="Confidence.Fail"/>. This test exercises AltTextQuality and LinkPurpose — both
/// heuristic by nature — against inputs designed to trigger every branch, and asserts none of
/// them ever produce a Fail. HeadingStructure is intentionally excluded: its checks are
/// deterministic structural facts and are permitted to report Fail.
/// </summary>
public sealed class HeuristicRulesNeverFailTests
{
    [Fact]
    public void AltTextQualityRule_NeverReportsFail_ForHeuristicJudgements()
    {
        var rule = new AltTextQualityRule();
        var images = new ContentFragment[]
        {
            new ImageFragment(TestLocations.OnMainBody(0), "src.jpg", "IMG_001.jpg", false),
            new ImageFragment(TestLocations.OnMainBody(1), "src.jpg", "image of a mountain range", false),
            new ImageFragment(TestLocations.OnMainBody(2), "src.jpg", new string('x', 300), false),
            new ImageFragment(TestLocations.OnMainBody(3), "src.jpg", "", false),
        };
        var document = new AuditDocument("content-1", images);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LinkPurposeRule_NeverReportsFail_ForHeuristicJudgements()
    {
        var rule = new LinkPurposeRule();
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/a", "click here", false),
            new LinkFragment(TestLocations.OnMainBody(1), "https://example.com", "https://example.com", false),
            new LinkFragment(TestLocations.OnMainBody(2), "", "", false),
            new LinkFragment(TestLocations.OnMainBody(3), "/x", "Report", false),
            new LinkFragment(TestLocations.OnMainBody(4), "/y", "Report", false),
        };
        var document = new AuditDocument("content-1", links);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HeadingStructureRule_MayReportFail_BecauseItsChecksAreDeterministic()
    {
        // Documents the deliberate exception: structural facts, not heuristics, may Fail.
        var rule = new HeadingStructureRule();
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 1, ""),
        };
        var document = new AuditDocument("content-1", headings);

        var findings = rule.Evaluate(document).ToList();

        Assert.Contains(findings, f => f.Confidence == Confidence.Fail);
    }
}

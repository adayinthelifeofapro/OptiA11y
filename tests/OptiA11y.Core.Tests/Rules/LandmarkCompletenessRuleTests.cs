using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LandmarkCompleteness;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LandmarkCompletenessRuleTests
{
    private readonly LandmarkCompletenessRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void MissingMainLandmark_IsNeedsReview()
    {
        var fragment = new LandmarkCompletenessFragment(TestLocations.OnMainBody(), false, Array.Empty<string>(), Array.Empty<string>());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ContentOutsideLandmarks_ProducesFinding()
    {
        var fragment = new LandmarkCompletenessFragment(TestLocations.OnMainBody(), true, new[] { "Stray text" }, Array.Empty<string>());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DuplicateUnlabelledLandmarks_ProducesFindingPerLandmark()
    {
        var fragment = new LandmarkCompletenessFragment(
            TestLocations.OnMainBody(),
            true,
            Array.Empty<string>(),
            new[] { "<nav> \"\"", "<nav> \"\"" });
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Equal(2, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.NeedsReview, f.Confidence));
    }
}

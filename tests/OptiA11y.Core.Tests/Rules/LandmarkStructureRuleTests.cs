using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LandmarkStructure;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LandmarkStructureRuleTests
{
    private readonly LandmarkStructureRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SingleLandmarkEmbeddedInContent_IsNeedsReview()
    {
        var fragment = new LandmarkFragment(TestLocations.OnMainBody(), "main", "main", IsExplicitRole: false, HasAccessibleName: false);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DuplicateUnlabelledLandmarks_ProducesNeedsReviewForEach()
    {
        var fragment1 = new LandmarkFragment(TestLocations.OnMainBody(), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: false);
        var fragment2 = new LandmarkFragment(TestLocations.OnMainBody(), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: false);
        var document = new AuditDocument("content-1", new ContentFragment[] { fragment1, fragment2 });

        var findings = _rule.Evaluate(document).ToList();

        // 2 findings for embedded-in-content, 2 for duplicate-unlabelled
        Assert.Equal(4, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.NeedsReview, f.Confidence));
    }

    [Fact]
    public void DuplicateLandmarksWithAccessibleNames_ProducesOnlyEmbeddedFindings()
    {
        var fragment1 = new LandmarkFragment(TestLocations.OnMainBody(), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: true);
        var fragment2 = new LandmarkFragment(TestLocations.OnMainBody(), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: true);
        var document = new AuditDocument("content-1", new ContentFragment[] { fragment1, fragment2 });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Equal(2, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.NeedsReview, f.Confidence));
    }
}

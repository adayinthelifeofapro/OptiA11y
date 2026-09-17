using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FocusAppearance;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FocusAppearanceRuleTests
{
    private readonly FocusAppearanceRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void BelowThreshold_IsNeedsReview()
    {
        var fragment = new FocusAppearanceFragment(TestLocations.OnMainBody(), "<button> \"Submit\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

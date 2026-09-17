using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.PointerTargetSpacing;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class PointerTargetSpacingRuleTests
{
    private readonly PointerTargetSpacingRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void InsufficientSpacing_IsNeedsReview()
    {
        var fragment = new PointerTargetSpacingFragment(TestLocations.OnMainBody(), "<button> \"X\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

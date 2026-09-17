using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FocusOrder;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FocusOrderRuleTests
{
    private readonly FocusOrderRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void FocusOrderDiverges_IsNeedsReview()
    {
        var fragment = new FocusOrderFragment(TestLocations.OnMainBody());
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

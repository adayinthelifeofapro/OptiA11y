using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.DraggingMovements;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class DraggingMovementsRuleTests
{
    private readonly DraggingMovementsRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DraggableWithoutAlternative_IsNeedsReview()
    {
        var fragment = new DraggingMovementsFragment(TestLocations.OnMainBody(), "<div> \"Slider\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

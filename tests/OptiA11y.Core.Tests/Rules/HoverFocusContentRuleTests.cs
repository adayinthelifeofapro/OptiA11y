using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.HoverFocusContent;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class HoverFocusContentRuleTests
{
    private readonly HoverFocusContentRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NotPersistentHoverContent_IsNeedsReview()
    {
        var fragment = new HoverFocusContentFragment(TestLocations.OnMainBody(), "<a> \"Info\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

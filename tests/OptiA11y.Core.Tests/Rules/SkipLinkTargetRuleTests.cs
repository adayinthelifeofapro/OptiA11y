using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SkipLinkTarget;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SkipLinkTargetRuleTests
{
    private readonly SkipLinkTargetRule _rule = new();

    [Fact]
    public void UnresolvedTarget_IsNeedsReview()
    {
        var skipLink = new SkipLinkFragment(TestLocations.OnMainBody(), "main-content", ResolvedWithinSameFragment: false);
        var document = new AuditDocument("content-1", new[] { skipLink });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ResolvedTarget_ProducesNoFinding()
    {
        var skipLink = new SkipLinkFragment(TestLocations.OnMainBody(), "main-content", ResolvedWithinSameFragment: true);
        var document = new AuditDocument("content-1", new[] { skipLink });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoSkipLinks_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }
}

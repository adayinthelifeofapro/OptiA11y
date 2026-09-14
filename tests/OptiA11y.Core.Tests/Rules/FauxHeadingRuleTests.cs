using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FauxHeading;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FauxHeadingRuleTests
{
    private readonly FauxHeadingRule _rule = new();

    [Fact]
    public void NoEmphasisBlocks_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void EmphasisBlock_IsNeedsReview()
    {
        var block = new EmphasisBlockFragment(TestLocations.OnMainBody(), "Getting started", 15);
        var document = new AuditDocument("content-1", new[] { block });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

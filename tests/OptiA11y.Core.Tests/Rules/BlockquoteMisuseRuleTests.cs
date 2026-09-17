using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.BlockquoteMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class BlockquoteMisuseRuleTests
{
    private readonly BlockquoteMisuseRule _rule = new();

    [Fact]
    public void NoCitation_IsNeedsReview()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "blockquote", "All prices in USD", HasCitation: false);
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void HasCitation_ProducesNoFinding()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "blockquote", "To be or not to be", HasCitation: true);
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void NonBlockquoteSpan_IsIgnored()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "sup", "1", HasCitation: false);
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

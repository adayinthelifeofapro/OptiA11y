using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.EmphasisMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class EmphasisMisuseRuleTests
{
    private readonly EmphasisMisuseRule _rule = new();

    [Fact]
    public void LongBoldRun_IsNeedsReview()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "b", new string('x', 130));
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ShortBoldRun_ProducesNoFinding()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "b", "Important!");
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void NonBoldItalicSpan_IsIgnored()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "blockquote", new string('x', 130));
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

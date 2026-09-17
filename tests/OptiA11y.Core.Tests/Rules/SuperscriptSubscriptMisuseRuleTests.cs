using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SuperscriptSubscriptMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SuperscriptSubscriptMisuseRuleTests
{
    private readonly SuperscriptSubscriptMisuseRule _rule = new();

    [Fact]
    public void LongSuperscriptRun_IsNeedsReview()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "sup", "This entire clause is wrapped in superscript for emphasis");
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ShortSuperscript_ProducesNoFinding()
    {
        var span = new MarkupSpanFragment(TestLocations.OnMainBody(), "sup", "th");
        var document = new AuditDocument("content-1", new[] { span });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

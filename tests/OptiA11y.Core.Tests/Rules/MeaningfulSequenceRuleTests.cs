using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.MeaningfulSequence;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class MeaningfulSequenceRuleTests
{
    private readonly MeaningfulSequenceRule _rule = new();

    [Fact]
    public void AbsolutelyPositionedText_IsNeedsReview()
    {
        var fragment = new PositionedContentFragment(TestLocations.OnMainBody(), "absolute", null, "Sidebar note");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void FloatedText_IsNeedsReview()
    {
        var fragment = new PositionedContentFragment(TestLocations.OnMainBody(), "static", "right", "Pull quote");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
    }
}

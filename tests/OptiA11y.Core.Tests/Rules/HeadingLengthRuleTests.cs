using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.HeadingLength;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class HeadingLengthRuleTests
{
    private readonly HeadingLengthRule _rule = new();

    [Fact]
    public void OverlongHeading_IsNeedsReview()
    {
        var heading = new HeadingFragment(TestLocations.OnMainBody(), 2, new string('x', 150));
        var document = new AuditDocument("content-1", new[] { heading });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ShortHeading_ProducesNoFinding()
    {
        var heading = new HeadingFragment(TestLocations.OnMainBody(), 2, "Our latest gear");
        var document = new AuditDocument("content-1", new[] { heading });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

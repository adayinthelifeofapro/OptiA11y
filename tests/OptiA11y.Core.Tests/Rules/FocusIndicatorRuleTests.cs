using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.FocusIndicator;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class FocusIndicatorRuleTests
{
    private readonly FocusIndicatorRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void MissingFocusIndicator_IsFail()
    {
        var fragment = new FocusIndicatorFragment(TestLocations.OnMainBody(), "<a> \"Skip to content\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }
}

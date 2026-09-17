using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.BlinkingContent;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class BlinkingContentRuleTests
{
    private readonly BlinkingContentRule _rule = new();

    [Fact]
    public void NoBlinkingContent_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void BlinkingElement_IsFail()
    {
        var fragment = new BlinkingContentFragment(TestLocations.OnMainBody(), "<blink> \"Hurry!\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
        Assert.Equal("2.2.2", findings[0].SuccessCriterion);
    }
}

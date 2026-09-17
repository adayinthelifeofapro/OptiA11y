using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.StatusMessages;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class StatusMessagesRuleTests
{
    private readonly StatusMessagesRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DynamicContentWithoutLiveRegion_IsNeedsReview()
    {
        var fragment = new StatusMessagesFragment(TestLocations.OnMainBody(), "<div> \"Saved\"");
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }
}

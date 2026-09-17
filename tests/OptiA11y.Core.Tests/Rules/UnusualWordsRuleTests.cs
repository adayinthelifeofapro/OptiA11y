using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.UnusualWords;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class UnusualWordsRuleTests
{
    private readonly UnusualWordsRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void JargonPhrase_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "We need to leverage synergy across teams.", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void PlainText_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "We need to work together across teams.", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

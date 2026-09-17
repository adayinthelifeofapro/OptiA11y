using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.PronunciationAmbiguity;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class PronunciationAmbiguityRuleTests
{
    private readonly PronunciationAmbiguityRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void Heteronym_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Please read the manual before use.", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void NoHeteronym_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Please review the manual before use.", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ReadingLevel;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ReadingLevelRuleTests
{
    private readonly ReadingLevelRule _rule = new();

    // A long, single run-on sentence full of multi-syllable words, well past the minimum word
    // count threshold - this should score as "very difficult" on the Flesch Reading Ease scale.
    private const string DifficultPassage =
        "Notwithstanding the aforementioned considerations, the organizational infrastructure " +
        "necessitates a comprehensive reevaluation of preexisting methodological frameworks in " +
        "order to accommodate the multifaceted, interdisciplinary requirements engendered by " +
        "the unprecedented technological transformations currently permeating substantially all " +
        "operational departments within the multinational corporate conglomerate, notwithstanding " +
        "considerable institutional resistance to procedural modifications.";

    [Fact]
    public void NoText_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void ShortText_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "This is short.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void VeryDifficultPassage_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), DifficultPassage, LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SimplePassage_ProducesNoFindings()
    {
        var simple = string.Join(" ", Enumerable.Repeat("The cat sat on the mat. It was a nice day.", 5));
        var text = new TextFragment(TestLocations.OnMainBody(), simple, LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }
}

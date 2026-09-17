using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.UnusualWords;

/// <summary>
/// Flags a small, curated set of common idioms and jargon phrases known to be difficult for
/// readers with cognitive disabilities, limited vocabulary, or who are unfamiliar with the
/// source language's figures of speech. This list is deliberately narrow and non-exhaustive -
/// it does not attempt to cover the full space of "unusual words" - so a false negative (an
/// unusual word not in the list) is expected, and this always reports
/// <see cref="Confidence.NeedsReview"/> so an author can judge whether a plain-language
/// alternative or glossary link is warranted.
/// </summary>
public sealed class UnusualWordsRule : IContentRule
{
    private static readonly string[] JargonPhrases =
    {
        "cutting-edge",
        "synergy",
        "low-hanging fruit",
        "move the needle",
        "boil the ocean",
        "circle back",
        "bandwidth",
        "paradigm shift",
        "table stakes",
        "hit the ground running",
    };

    public string RuleId => "unusual-words";

    public string SuccessCriterion => "3.1.3";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            foreach (var phrase in JargonPhrases)
            {
                if (text.Text.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new Finding(
                        RuleId,
                        SuccessCriterion,
                        Level,
                        Severity.Minor,
                        Confidence.NeedsReview,
                        text.Location,
                        $"The text \"{text.Text}\" contains the phrase \"{phrase}\", which is jargon or an idiom that may be difficult for some readers to understand. Consider a plain-language alternative or a glossary link.");
                }
            }
        }
    }
}

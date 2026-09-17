using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PronunciationAmbiguity;

/// <summary>
/// Flags a small, curated set of English heteronyms - words spelled the same but pronounced
/// differently depending on meaning (e.g. "read", "lead", "wind", "close", "live", "tear") -
/// with no surrounding context (like a `&lt;ruby&gt;` annotation) that would help a screen
/// reader's text-to-speech engine or a reader unfamiliar with the word choose the correct
/// pronunciation. Whether a given occurrence is actually ambiguous in context is an editorial
/// judgement call the parser cannot make, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class PronunciationAmbiguityRule : IContentRule
{
    private static readonly string[] Heteronyms =
    {
        "read",
        "lead",
        "wind",
        "close",
        "live",
        "tear",
        "wound",
        "bass",
        "minute",
        "object",
    };

    private static readonly Regex WordBoundary = new(@"\b[a-zA-Z]+\b", RegexOptions.Compiled);

    public string RuleId => "pronunciation-ambiguity";

    public string SuccessCriterion => "3.1.6";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            foreach (Match match in WordBoundary.Matches(text.Text))
            {
                var word = match.Value.ToLowerInvariant();
                if (!Heteronyms.Contains(word))
                {
                    continue;
                }

                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The text \"{text.Text}\" contains \"{match.Value}\", a word whose pronunciation depends on meaning (a heteronym). Screen readers may guess the wrong pronunciation - consider rephrasing or adding a pronunciation annotation if the meaning is ambiguous in context.");

                break;
            }
        }
    }
}

using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ReadingLevel;

/// <summary>
/// Estimates reading ease with a Flesch Reading Ease approximation and flags passages that read
/// as very difficult, which WCAG 3.1.5 asks authors to avoid (or supplement with a simpler
/// summary) for content aimed at a general audience. A readability formula is a statistical
/// proxy, not a judgement of whether THIS content needs to be simple, so this is always
/// NeedsReview.
/// </summary>
public sealed class ReadingLevelRule : IContentRule
{
    private const int MinimumWordCount = 40;
    private const double VeryDifficultThreshold = 30.0;

    private static readonly Regex WordPattern = new(@"[A-Za-z]+", RegexOptions.Compiled);
    private static readonly Regex SentencePattern = new(@"[.!?]+", RegexOptions.Compiled);
    private static readonly Regex VowelGroupPattern = new(@"[aeiouy]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string RuleId => "reading-level";

    public string SuccessCriterion => "3.1.5";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            var words = WordPattern.Matches(text.Text).Select(m => m.Value).ToList();
            if (words.Count < MinimumWordCount)
            {
                continue;
            }

            var sentenceCount = Math.Max(1, SentencePattern.Matches(text.Text).Count);
            var syllableCount = words.Sum(CountSyllables);

            var score = 206.835
                - 1.015 * ((double)words.Count / sentenceCount)
                - 84.6 * ((double)syllableCount / words.Count);

            if (score < VeryDifficultThreshold)
            {
                var sample = text.Text.Length > 80 ? text.Text[..80] : text.Text;
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Info,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The passage starting \"{sample}\" scores {score:F0} on the Flesch Reading Ease scale (below {VeryDifficultThreshold:F0} is considered very difficult, roughly college-graduate level). Consider shorter sentences and simpler words, or a plain-language summary.");
            }
        }
    }

    private static int CountSyllables(string word)
    {
        var count = VowelGroupPattern.Matches(word).Count;
        return Math.Max(1, count);
    }
}

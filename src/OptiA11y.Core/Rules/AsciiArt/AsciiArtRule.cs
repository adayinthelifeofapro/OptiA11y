using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AsciiArt;

/// <summary>
/// Flags lines made up mostly of repeated punctuation (e.g. "----------", "======", "~~~~~~~~"),
/// which are typically used as visual dividers or crude "ASCII art" rather than meaningful text.
/// Screen readers either read every character aloud ("dash dash dash dash...") or, depending on
/// verbosity settings, skip the content, so neither behaviour conveys the intended visual effect.
/// Distinguishing a deliberate divider from other punctuation-heavy but meaningful text (e.g. a
/// code sample) is a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class AsciiArtRule : IContentRule
{
    private static readonly Regex PunctuationRun = new(@"[-=~_*#+.]{8,}", RegexOptions.Compiled);

    public string RuleId => "ascii-art";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            var match = PunctuationRun.Match(text.Text);
            if (!match.Success)
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
                $"The text \"{text.Text}\" contains a run of repeated punctuation (\"{match.Value}\"), which looks like a visual divider or ASCII art rather than meaningful content. Screen readers either read each character aloud or skip it, so the visual effect is lost either way - consider a semantic `<hr>` or removing it.");
        }
    }
}

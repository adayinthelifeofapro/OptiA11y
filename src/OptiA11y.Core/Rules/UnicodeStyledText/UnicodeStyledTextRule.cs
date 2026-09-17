using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.UnicodeStyledText;

/// <summary>
/// Flags text that uses Unicode "fancy font" characters (Mathematical Alphanumeric Symbols, or
/// fullwidth forms) instead of plain text with real formatting. These lookalike glyphs are
/// frequently used on social media to fake bold/italic styling, but screen readers either read
/// them letter-by-letter, mispronounce them, or skip them entirely, and they aren't found by
/// in-page search. Because the affected code points are members of two small, fixed Unicode
/// blocks with no legitimate use in body text, this is a deterministic structural fact rather
/// than a judgement call, so it reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class UnicodeStyledTextRule : IContentRule
{
    // Mathematical Alphanumeric Symbols (bold/italic/script/fraktur/double-struck/sans-serif/
    // monospace lookalike letters and digits) and the Halfwidth and Fullwidth Forms block.
    private static readonly Regex StyledCodePoints = new(@"[\uD835][\uDC00-\uDFFF]|[\uFF00-\uFFEF]", RegexOptions.Compiled);

    public string RuleId => "unicode-styled-text";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            var match = StyledCodePoints.Match(text.Text);
            if (!match.Success)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.Fail,
                text.Location,
                $"The text \"{text.Text}\" contains Unicode \"fancy font\" characters (mathematical alphanumeric or fullwidth symbols) instead of plain text with real formatting. Screen readers may mispronounce, skip, or spell out these characters, and they won't match ordinary in-page search.");
        }
    }
}

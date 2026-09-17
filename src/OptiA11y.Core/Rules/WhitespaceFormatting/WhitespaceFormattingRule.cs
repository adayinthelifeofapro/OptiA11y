using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.WhitespaceFormatting;

/// <summary>
/// Flags text that uses repeated non-breaking spaces or long runs of regular spaces to fake
/// visual layout (indentation, alignment, or column spacing) rather than real CSS or table
/// markup. Screen readers may announce each space, collapse them unpredictably, or ignore the
/// intended alignment entirely, so the visual effect doesn't reach assistive technology users.
/// Whether a given run of whitespace is deliberate layout-faking versus an incidental copy-paste
/// artifact is a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class WhitespaceFormattingRule : IContentRule
{
    private static readonly Regex RepeatedNonBreakingSpace = new("\u00A0{2,}", RegexOptions.Compiled);

    private static readonly Regex RepeatedRegularSpace = new(" {3,}", RegexOptions.Compiled);

    public string RuleId => "whitespace-formatting";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            if (RepeatedNonBreakingSpace.IsMatch(text.Text))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The text \"{text.Text}\" contains repeated non-breaking spaces, which looks like it's being used to fake indentation or alignment. Consider using CSS for layout instead, since assistive technology may announce or collapse these spaces unpredictably.");
            }
            else if (RepeatedRegularSpace.IsMatch(text.Text))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The text \"{text.Text}\" contains a long run of spaces, which looks like it's being used to fake column alignment or indentation. Consider using CSS or a real table instead.");
            }
        }
    }
}

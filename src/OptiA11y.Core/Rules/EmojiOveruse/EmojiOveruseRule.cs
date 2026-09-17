using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.EmojiOveruse;

/// <summary>
/// Flags runs of 3 or more consecutive emoji, or emoji used as a list-marker-like prefix at the
/// start of text. Screen readers announce every emoji by its full name (e.g. "party popper,
/// party popper, party popper"), which becomes noisy and disorienting in a repeated run, and
/// using an emoji as a makeshift bullet doesn't expose the same list semantics as a real list
/// item. Whether a given emoji run is decorative noise or a deliberate expressive choice is a
/// judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class EmojiOveruseRule : IContentRule
{
    // A broad-but-common emoji range covering emoticons, symbols/pictographs, transport, and
    // the supplemental symbols/pictographs blocks. Not exhaustive of every Unicode emoji, but
    // covers the overwhelming majority seen in authored content.
    private static readonly Regex EmojiRun = new(
        @"[\uD83C-\uD83E][\uDC00-\uDFFF](\s*[\uD83C-\uD83E][\uDC00-\uDFFF]){2,}",
        RegexOptions.Compiled);

    private static readonly Regex LeadingEmojiBullet = new(
        @"^\s*[\uD83C-\uD83E][\uDC00-\uDFFF]\s+\S",
        RegexOptions.Compiled);

    public string RuleId => "emoji-overuse";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            if (EmojiRun.IsMatch(text.Text))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The text \"{text.Text}\" contains a run of consecutive emoji. Screen readers announce each one by its full name, which can become noisy - consider reducing repeated emoji to a single instance.");
            }
            else if (LeadingEmojiBullet.IsMatch(text.Text))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    text.Location,
                    $"The text \"{text.Text}\" starts with an emoji used as a bullet/marker. This doesn't expose real list semantics to assistive technology - consider using an actual list element instead.");
            }
        }
    }
}

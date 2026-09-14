using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TextReadability;

/// <summary>
/// Flags text styling known to hinder screen-reader/low-vision users: fully justified text
/// (which creates uneven spacing that some readers find harder to parse, per WCAG 1.4.8) and
/// font sizes small enough to be difficult to read. Both checks compare explicitly authored
/// inline styles against fixed thresholds, so those report <see cref="Confidence.Fail"/>.
///
/// Also flags long runs of upper-case text ("shouting"), which some screen readers spell out
/// letter-by-letter rather than reading as words. Whether a given run is emphasis, an acronym,
/// or genuinely meant to be read as words is a judgement call, so that check reports
/// <see cref="Confidence.NeedsReview"/> instead.
/// </summary>
public sealed class TextReadabilityRule : IContentRule
{
    private const double MinimumFontSizePx = 12;

    // A run of 12+ upper-case letters (allowing spaces/hyphens between words) is a stronger
    // signal of "shouting" than a short acronym like "NASA" or "WCAG", which this intentionally
    // does not flag.
    private static readonly Regex AllCapsRun = new(@"\b[A-Z][A-Z\s\-]{11,}[A-Z]\b", RegexOptions.Compiled);

    public string RuleId => "text-readability";

    public string SuccessCriterion => "1.4.8";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            var match = AllCapsRun.Match(text.Text);
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
                $"The text \"{match.Value}\" is set entirely in capital letters. Some screen readers announce long capitalized runs letter-by-letter rather than as words. Consider normal capitalization and using CSS text-transform for visual emphasis instead.");
        }

        foreach (var fragment in document.Get<TextStyleFragment>())
        {
            if (string.Equals(fragment.TextAlign, "justify", StringComparison.OrdinalIgnoreCase))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.Fail,
                    fragment.Location,
                    $"Text \"{fragment.SampleText}\" is fully justified, which creates uneven word spacing ('rivers') that can be harder to read for users with low vision or cognitive disabilities.");
            }

            if (fragment.FontSizePx is < MinimumFontSizePx)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.Fail,
                    fragment.Location,
                    $"Text \"{fragment.SampleText}\" is set to {fragment.FontSizePx}px, below the {MinimumFontSizePx}px generally considered readable. Users with low vision may struggle without zooming.");
            }
        }
    }
}

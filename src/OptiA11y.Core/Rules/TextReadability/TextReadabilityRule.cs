using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TextReadability;

/// <summary>
/// Flags text styling known to hinder screen-reader/low-vision users: fully justified text
/// (which creates uneven spacing that some readers find harder to parse, per WCAG 1.4.8) and
/// font sizes small enough to be difficult to read. Both checks compare explicitly authored
/// inline styles against fixed thresholds, so this rule reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class TextReadabilityRule : IContentRule
{
    private const double MinimumFontSizePx = 12;

    public string RuleId => "text-readability";

    public string SuccessCriterion => "1.4.8";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
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

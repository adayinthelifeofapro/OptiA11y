using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ColorContrast;

/// <summary>
/// Evaluates styled text for sufficient contrast against its background, per WCAG 1.4.3.
/// The contrast ratio and thresholds (4.5:1 for normal text, 3:1 for large text) are a
/// deterministic mathematical calculation over explicitly authored colors, so this rule reports
/// <see cref="Confidence.Fail"/> - except when <see cref="ColorContrastFragment.BackgroundIsImage"/>
/// is set (only ever true from rendered-style enrichment): the sampled background color there is
/// just whatever solid color sits behind an image or gradient, not what's actually visible, so a
/// ratio computed against it isn't reliable enough to Fail on - that case instead reports
/// <see cref="Confidence.NeedsReview"/>, asking for a manual check.
/// </summary>
public sealed class ColorContrastRule : IContentRule
{
    private const double NormalTextThreshold = 4.5;
    private const double LargeTextThreshold = 3.0;

    public string RuleId => "color-contrast";

    public string SuccessCriterion => "1.4.3";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<ColorContrastFragment>())
        {
            var threshold = fragment.IsLargeText ? LargeTextThreshold : NormalTextThreshold;

            if (fragment.BackgroundIsImage)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    fragment.Location,
                    $"Text \"{fragment.SampleText}\" sits over an image or gradient background rather than a flat color. Its computed contrast ratio ({fragment.ContrastRatio:F2}:1) isn't reliable - check contrast manually against the busiest part of the image behind it.");
                continue;
            }

            if (fragment.ContrastRatio >= threshold)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                fragment.Location,
                $"Text \"{fragment.SampleText}\" has a contrast ratio of {fragment.ContrastRatio:F2}:1 between {fragment.ForegroundColor} and {fragment.BackgroundColor}, below the required {threshold:F1}:1 for {(fragment.IsLargeText ? "large" : "normal-sized")} text.");
        }
    }
}

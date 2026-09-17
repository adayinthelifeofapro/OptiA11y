using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ContrastEnhanced;

/// <summary>
/// Evaluates styled text against the AAA enhanced-contrast thresholds (7:1 normal, 4.5:1 large) -
/// WCAG 1.4.6. Reuses <see cref="ColorContrastFragment"/> from the same inline-style enrichment
/// as <c>color-contrast</c>, applying the stricter AAA thresholds instead of the AA ones. This is
/// a deterministic mathematical calculation over explicitly authored colors, so it reports
/// <see cref="Confidence.Fail"/> - except when <see cref="ColorContrastFragment.BackgroundIsImage"/>
/// is set, matching <c>color-contrast</c>'s precedent that a ratio computed against an
/// image/gradient's fallback solid color isn't reliable enough to Fail on.
/// </summary>
public sealed class ContrastEnhancedRule : IContentRule
{
    private const double NormalTextThreshold = 7.0;
    private const double LargeTextThreshold = 4.5;

    public string RuleId => "contrast-enhanced";

    public string SuccessCriterion => "1.4.6";

    public WcagLevel Level => WcagLevel.AAA;

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
                    $"Text \"{fragment.SampleText}\" sits over an image or gradient background rather than a flat color. Its computed contrast ratio ({fragment.ContrastRatio:F2}:1) isn't reliable enough to check against the AAA {threshold:F1}:1 threshold - verify manually.");
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
                Severity.Minor,
                Confidence.Fail,
                fragment.Location,
                $"Text \"{fragment.SampleText}\" has a contrast ratio of {fragment.ContrastRatio:F2}:1 between {fragment.ForegroundColor} and {fragment.BackgroundColor}, below the AAA enhanced-contrast requirement of {threshold:F1}:1 for {(fragment.IsLargeText ? "large" : "normal-sized")} text.");
        }
    }
}

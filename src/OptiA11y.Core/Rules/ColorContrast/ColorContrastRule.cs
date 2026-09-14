using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ColorContrast;

/// <summary>
/// Evaluates inline-styled text for sufficient contrast against its background, per WCAG 1.4.3.
/// The contrast ratio and thresholds (4.5:1 for normal text, 3:1 for large text) are a
/// deterministic mathematical calculation over explicitly authored colors, so this rule reports
/// <see cref="Confidence.Fail"/>. Note this only inspects inline <c>style</c> attributes; colors
/// set via external stylesheets are outside what a content-level audit can see.
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

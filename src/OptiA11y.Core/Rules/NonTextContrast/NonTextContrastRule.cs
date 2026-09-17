using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.NonTextContrast;

/// <summary>
/// Evaluates inline-styled borders and icon fills for the 3:1 non-text contrast minimum against
/// their effective background (WCAG 1.4.11). The contrast ratio is a deterministic mathematical
/// calculation over explicitly authored colors, so this reports <see cref="Confidence.Fail"/> -
/// except when the background color is the fragment's documented white-default fallback (no
/// ancestor set an explicit background), in which case the assumption itself is unverified, so
/// this reports <see cref="Confidence.NeedsReview"/> instead.
/// </summary>
public sealed class NonTextContrastRule : IContentRule
{
    private const double Threshold = 3.0;
    private const string AssumedDefaultBackground = "#ffffff";

    public string RuleId => "non-text-contrast";

    public string SuccessCriterion => "1.4.11";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<NonTextContrastFragment>())
        {
            if (fragment.ContrastRatio >= Threshold)
            {
                continue;
            }

            var isAssumedBackground = string.Equals(fragment.BackgroundColor, AssumedDefaultBackground, StringComparison.OrdinalIgnoreCase);

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                isAssumedBackground ? Confidence.NeedsReview : Confidence.Fail,
                fragment.Location,
                isAssumedBackground
                    ? $"{fragment.ElementDescription} has a computed contrast ratio of {fragment.ContrastRatio:F2}:1 against an assumed white background (no ancestor sets an explicit background-color) - below the required {Threshold:F1}:1. Confirm the actual background and contrast."
                    : $"{fragment.ElementDescription} has a contrast ratio of {fragment.ContrastRatio:F2}:1 between {fragment.ForegroundColor} and {fragment.BackgroundColor}, below the required {Threshold:F1}:1 for non-text content.");
        }
    }
}

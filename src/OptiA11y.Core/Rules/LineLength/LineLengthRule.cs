using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LineLength;

/// <summary>
/// Flags an inline-styled text block whose width is wide enough to produce very long lines
/// (WCAG 1.4.8, AAA recommends no more than ~80 characters per line). Pixel width is an
/// approximation for character count, not an exact measurement (it depends on font size and
/// family), so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LineLengthRule : IContentRule
{
    private const double WidthThresholdPx = 900;

    public string RuleId => "line-length";

    public string SuccessCriterion => "1.4.8";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<TextStyleFragment>())
        {
            if (fragment.WidthPx is not > WidthThresholdPx)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"Text \"{fragment.SampleText}\" is in a block styled to {fragment.WidthPx:F0}px wide, which may produce lines longer than the AAA-recommended ~80 characters. Consider constraining the width (e.g. to 80ch or ~700px).");
        }
    }
}

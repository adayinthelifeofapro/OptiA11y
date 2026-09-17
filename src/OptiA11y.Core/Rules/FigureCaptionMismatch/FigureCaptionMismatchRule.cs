using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FigureCaptionMismatch;

/// <summary>
/// Flags a figure whose caption and image alt text are identical (ignoring case/whitespace).
/// A screen reader announces the alt text and then immediately reads the caption, so an exact
/// duplicate is redundant — but whether that's actually a problem depends on how long/detailed
/// the text is, so this is a <see cref="Confidence.NeedsReview"/> judgement, not a defect.
/// </summary>
public sealed class FigureCaptionMismatchRule : IContentRule
{
    public string RuleId => "figure-caption-mismatch";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var figure in document.Get<FigureFragment>())
        {
            if (string.IsNullOrWhiteSpace(figure.AltText) || string.IsNullOrWhiteSpace(figure.Caption))
            {
                continue;
            }

            var alt = figure.AltText.Trim();
            var caption = figure.Caption.Trim();

            if (string.Equals(alt, caption, StringComparison.OrdinalIgnoreCase))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    figure.Location,
                    "This figure's alt text is identical to its caption, so a screen reader announces the same text twice. Consider marking the image decorative (empty alt) since the caption already describes it, or differentiate the two.");
            }
        }
    }
}

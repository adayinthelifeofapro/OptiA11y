using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DecorativeImageMisuse;

/// <summary>
/// Flags a figure whose image has empty alt text (marking it decorative) despite the figure
/// having its own caption. A captioned figure is, by convention, presenting the image as content
/// worth describing — an empty alt in that context is a common authoring slip rather than a
/// deliberate decorative marking, but it cannot be asserted as a defect, so this is
/// <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class DecorativeImageMisuseRule : IContentRule
{
    public string RuleId => "decorative-image-misuse";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var figure in document.Get<FigureFragment>())
        {
            if (figure.AltText is { Length: 0 } && !string.IsNullOrWhiteSpace(figure.Caption))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    figure.Location,
                    "This figure has a caption but its image is marked decorative (empty alt). If the image adds information beyond the caption, give it a real description instead of marking it decorative.");
            }
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ComplexImageDescription;

/// <summary>
/// Flags images whose filename or short alt text suggests a chart, graph, diagram, or
/// infographic — content that typically needs more than a one-line description to be usable
/// non-visually. Whether the existing alt text is actually sufficient is an editorial judgement
/// the tool cannot make, so this is always <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class ComplexImageDescriptionRule : IContentRule
{
    private const int ShortAltMaxLength = 80;

    private static readonly string[] ComplexImageKeywords =
    {
        "chart", "graph", "diagram", "infographic", "flowchart", "plot", "timeline", "map"
    };

    public string RuleId => "complex-image-description";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var image in document.Get<ImageFragment>())
        {
            if (string.IsNullOrEmpty(image.AltText) || image.AltText.Length > ShortAltMaxLength)
            {
                continue;
            }

            var haystack = $"{image.Src} {image.AltText}".ToLowerInvariant();
            if (ComplexImageKeywords.Any(keyword => haystack.Contains(keyword, StringComparison.Ordinal)))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    image.Location,
                    "This image's filename or alt text suggests a chart, graph, or diagram, but the alt text is short. Complex images usually need a longer description or a data table/text equivalent nearby, not just a one-line label.");
            }
        }
    }
}

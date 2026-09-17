using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LandmarkStructure;

/// <summary>
/// Flags two situations: a landmark role (<c>main</c>, <c>banner</c>, <c>contentinfo</c>, etc.)
/// embedded directly in a content property's markup - which usually belongs to the page template
/// rather than editorial content - and multiple landmarks of the same role on the same page with
/// no distinguishing accessible name, which forces screen reader users to guess which is which.
/// Whether a landmark embedded in content is actually a mistake (as opposed to a legitimate
/// content-authored region) is an editorial judgement call this rule cannot make with certainty,
/// so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LandmarkStructureRule : IContentRule
{
    public string RuleId => "landmark-structure";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var landmarks = document.Get<LandmarkFragment>().ToList();

        foreach (var landmark in landmarks)
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                landmark.Location,
                $"This <{landmark.TagName}> is a \"{landmark.Role}\" landmark embedded within content markup. Landmarks usually belong in the page template rather than editorial content - verify this isn't duplicating or conflicting with the page's own layout landmarks.");
        }

        foreach (var group in landmarks.GroupBy(l => l.Role, StringComparer.OrdinalIgnoreCase))
        {
            var unlabelled = group.Where(l => !l.HasAccessibleName).ToList();
            if (unlabelled.Count < 2)
            {
                continue;
            }

            foreach (var landmark in unlabelled)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    landmark.Location,
                    $"This <{landmark.TagName}> is one of multiple \"{landmark.Role}\" landmarks in this content with no aria-label/aria-labelledby to distinguish them. Screen reader users navigating by landmark will not be able to tell them apart.");
            }
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LandmarkCompleteness;

/// <summary>
/// Flags gaps in landmark-region completeness for the rendered page (WCAG 1.3.1): a missing or
/// duplicated <c>main</c> landmark, visible content outside every landmark region, or landmarks
/// sharing a role with no distinguishing accessible name. <see cref="LandmarkCompletenessFragment"/>
/// is only ever produced by the rendered-style enrichment slice. Whether a given gap is actually
/// disorienting for assistive-technology users depends on the page's specific structure, so this
/// always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LandmarkCompletenessRule : IContentRule
{
    public string RuleId => "landmark-completeness";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var landmarks in document.Get<LandmarkCompletenessFragment>())
        {
            if (!landmarks.HasMainLandmark)
            {
                yield return NeedsReview(landmarks.Location, "This page does not have exactly one <main> landmark. Screen reader users rely on a single main landmark to jump directly to the primary content.");
            }

            if (landmarks.ContentOutsideAnyLandmark.Count > 0)
            {
                yield return NeedsReview(landmarks.Location, $"This page has visible content outside every landmark region (e.g. \"{landmarks.ContentOutsideAnyLandmark[0]}\"). Screen reader users navigating by landmark may miss it entirely.");
            }

            foreach (var description in landmarks.DuplicateUnlabelledLandmarkDescriptions)
            {
                yield return NeedsReview(landmarks.Location, $"{description} shares its role with another landmark on the page and has no aria-label/aria-labelledby to distinguish it. Screen reader users navigating by landmark may not be able to tell them apart.");
            }
        }
    }

    private Finding NeedsReview(SourceLocation location, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.NeedsReview,
        location,
        message);
}

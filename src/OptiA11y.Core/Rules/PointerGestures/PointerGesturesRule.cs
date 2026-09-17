using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PointerGestures;

/// <summary>
/// Flags an element whose interaction handlers imply a multipoint or path-based gesture with no
/// simpler single-pointer alternative detected (WCAG 2.5.1). <see cref="PointerGesturesFragment"/>
/// is only ever produced by the rendered-style enrichment slice. Confirming the absence of any
/// alternative elsewhere on the page is not fully verifiable, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class PointerGesturesRule : IContentRule
{
    public string RuleId => "pointer-gestures";

    public string SuccessCriterion => "2.5.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<PointerGesturesFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.NeedsReview,
                element.Location,
                $"{element.ElementDescription} appears to require a multipoint or path-based gesture (e.g. pinch or swipe). Confirm a single-pointer alternative (like buttons) is available for users who can't perform complex gestures.");
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.Motion;

/// <summary>
/// Flags an element with a CSS animation that repeats indefinitely and starts automatically
/// (WCAG 2.2.2). <see cref="MotionFragment"/> is only ever produced by the rendered-style
/// enrichment slice (<c>OptiA11y.Rendering</c>). 2.2.2 allows an "essential" exception this rule
/// has no way to verify, so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class MotionRule : IContentRule
{
    public string RuleId => "motion";

    public string SuccessCriterion => "2.2.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<MotionFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                element.Location,
                $"{element.ElementDescription} has a CSS animation that repeats indefinitely and starts automatically. Unless the motion is essential, provide a way to pause, stop, or hide it.");
        }
    }
}

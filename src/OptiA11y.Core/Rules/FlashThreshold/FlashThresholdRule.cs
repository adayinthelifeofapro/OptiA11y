using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FlashThreshold;

/// <summary>
/// Flags an element whose animation/transition timing implies flashes more frequent than the
/// WCAG 2.3.1 general flash threshold. <see cref="FlashThresholdFragment"/> is only ever
/// produced by the rendered-style enrichment slice from an animation-timing heuristic, not true
/// sampled luminance analysis (which would require frame-by-frame screenshot comparison), so
/// this always reports <see cref="Confidence.NeedsReview"/> even though a genuinely measured
/// flash would be a Fail-level violation.
/// </summary>
public sealed class FlashThresholdRule : IContentRule
{
    public string RuleId => "flash-threshold";

    public string SuccessCriterion => "2.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var flash in document.Get<FlashThresholdFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.NeedsReview,
                flash.Location,
                "An element on this page animates faster than roughly 3 times per second. This timing alone suggests it could exceed the general flash threshold - confirm with real luminance sampling, since a genuine flash risk can trigger seizures.");
        }
    }
}

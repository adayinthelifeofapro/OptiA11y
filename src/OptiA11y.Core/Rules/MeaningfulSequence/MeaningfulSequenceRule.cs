using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.MeaningfulSequence;

/// <summary>
/// Flags inline <c>position: absolute</c>/<c>fixed</c> or <c>float</c> styling on text-bearing
/// elements — a signal that the visual reading order may diverge from the DOM order a screen
/// reader follows. This can be entirely legitimate (a pull quote, a sidebar), so this rule
/// reports <see cref="Confidence.NeedsReview"/> only, prompting a human check rather than
/// asserting a defect.
/// </summary>
public sealed class MeaningfulSequenceRule : IContentRule
{
    public string RuleId => "meaningful-sequence";

    public string SuccessCriterion => "1.3.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<PositionedContentFragment>())
        {
            var mechanism = fragment.Position is "absolute" or "fixed" ? $"position: {fragment.Position}" : $"float: {fragment.Float}";

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"This text (\"{fragment.SampleText}\") uses inline {mechanism}, which can make its visual position diverge from the order a screen reader announces it in. Confirm the reading order still makes sense with styling removed.");
        }
    }
}

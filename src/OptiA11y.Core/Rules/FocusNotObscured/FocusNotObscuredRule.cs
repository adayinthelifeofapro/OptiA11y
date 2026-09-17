using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FocusNotObscured;

/// <summary>
/// Flags a focusable element that is at least partially covered by a sticky/fixed element once
/// it receives keyboard focus (WCAG 2.4.11). <see cref="FocusNotObscuredFragment"/> is only ever
/// produced by the rendered-style enrichment slice from a direct bounding-box overlap
/// measurement, so a reported overlap is a structural fact, hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class FocusNotObscuredRule : IContentRule
{
    public string RuleId => "focus-not-obscured";

    public string SuccessCriterion => "2.4.11";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var obscured in document.Get<FocusNotObscuredFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                obscured.Location,
                $"{obscured.ElementDescription} is covered by a sticky header or footer once focused. Keyboard users may not be able to see it while it's active - adjust scroll-margin or sticky element sizing so focused elements remain visible.");
        }
    }
}

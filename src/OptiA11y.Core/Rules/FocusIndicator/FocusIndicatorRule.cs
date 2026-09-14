using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FocusIndicator;

/// <summary>
/// Flags a focusable element whose rendered appearance doesn't change at all when it receives
/// keyboard focus (WCAG 2.4.7). <see cref="FocusIndicatorFragment"/> is only ever produced by the
/// rendered-style enrichment slice (<c>OptiA11y.Rendering</c>), which compares the element's
/// computed style before and after focusing it across several properties (outline, box-shadow,
/// background, border) - a genuinely measured absence of any visual change, so this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class FocusIndicatorRule : IContentRule
{
    public string RuleId => "focus-indicator";

    public string SuccessCriterion => "2.4.7";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<FocusIndicatorFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                element.Location,
                $"{element.ElementDescription} shows no visible change (outline, box-shadow, background, or border) when it receives keyboard focus. Keyboard users can't tell where focus is.");
        }
    }
}

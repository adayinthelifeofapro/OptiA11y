using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.KeyboardTrap;

/// <summary>
/// Flags a component that sequential Tab-key presses could not escape within a bounded number
/// of iterations (WCAG 2.1.2). <see cref="KeyboardTrapFragment"/> is only ever produced by the
/// rendered-style enrichment slice (<c>OptiA11y.Rendering</c>) from a real keyboard simulation
/// against a live page, so a reported trap is a structural fact, hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class KeyboardTrapRule : IContentRule
{
    public string RuleId => "keyboard-trap";

    public string SuccessCriterion => "2.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var trap in document.Get<KeyboardTrapFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.Fail,
                trap.Location,
                "Keyboard focus could not escape a component on this page after repeated Tab presses. Keyboard-only users can become permanently stuck here - ensure every component can be exited with Tab, Shift+Tab, or Escape.");
        }
    }
}

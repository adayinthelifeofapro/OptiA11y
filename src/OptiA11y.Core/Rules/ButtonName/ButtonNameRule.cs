using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ButtonName;

/// <summary>
/// Evaluates buttons (native &lt;button&gt;, submit/reset/button/image &lt;input&gt;) for an
/// accessible name. Whether a button has visible text, a value attribute, or an aria-label is a
/// deterministic structural fact, so this rule reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class ButtonNameRule : IContentRule
{
    public string RuleId => "button-name";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var button in document.Get<ButtonFragment>())
        {
            if (!button.HasAccessibleName)
            {
                yield return Fail(button, $"This {button.ControlType} has no accessible name (no visible text, value, or aria-label). Screen reader users will hear only \"button\".");
            }
        }
    }

    private Finding Fail(ButtonFragment button, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Critical,
        Confidence.Fail,
        button.Location,
        message);
}

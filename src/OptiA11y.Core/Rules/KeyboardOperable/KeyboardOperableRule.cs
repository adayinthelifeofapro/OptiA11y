using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.KeyboardOperable;

/// <summary>
/// Flags an element with a click handler that is neither natively focusable/operable nor
/// exposes a <c>tabindex</c>, so keyboard users cannot reach or activate it (WCAG 2.1.1).
/// <see cref="KeyboardOperableFragment"/> is only ever produced by the rendered-style enrichment
/// slice from a direct DOM inspection, so this is a structural fact, hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class KeyboardOperableRule : IContentRule
{
    public string RuleId => "keyboard-operable";

    public string SuccessCriterion => "2.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<KeyboardOperableFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.Fail,
                element.Location,
                $"{element.ElementDescription} has a click handler but is not a natively focusable element and has no tabindex, so keyboard-only users cannot reach or activate it. Make it a button/link, or add a tabindex and keydown handler.");
        }
    }
}

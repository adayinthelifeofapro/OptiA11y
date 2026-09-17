using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AriaHiddenFocusable;

/// <summary>
/// Flags an element with <c>aria-hidden="true"</c> that contains a natively focusable
/// descendant. This is always invalid regardless of authoring intent: keyboard users can still
/// tab into the hidden content, but assistive technology won't announce it, creating a
/// confusing "phantom focus" experience. Deterministic structural fact, so this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class AriaHiddenFocusableRule : IContentRule
{
    public string RuleId => "aria-hidden-focusable";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            if (!fragment.IsAriaHiddenWithFocusableDescendant)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                fragment.Location,
                $"This <{fragment.TagName}> is aria-hidden=\"true\" but contains a focusable element. Keyboard users can still tab into it while assistive technology skips over it entirely.");
        }
    }
}

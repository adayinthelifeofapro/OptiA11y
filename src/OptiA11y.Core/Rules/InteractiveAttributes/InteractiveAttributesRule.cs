using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.InteractiveAttributes;

/// <summary>
/// Evaluates focus-management and keyboard-access attributes for deterministic problems:
/// natively interactive elements hidden from assistive technology via aria-hidden, positive
/// tabindex values that break natural reading/focus order, and duplicate access keys. All of
/// these are structural facts, so this rule reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class InteractiveAttributesRule : IContentRule
{
    public string RuleId => "interactive-attributes";

    public string SuccessCriterion => "2.4.3";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var seenAccessKeys = new Dictionary<string, InteractiveAttributesFragment>(StringComparer.OrdinalIgnoreCase);

        foreach (var fragment in document.Get<InteractiveAttributesFragment>())
        {
            if (fragment.AriaHidden && fragment.IsNativelyInteractive)
            {
                yield return Fail(
                    fragment,
                    $"This <{fragment.TagName}> is aria-hidden=\"true\" but is natively interactive/focusable. Keyboard and screen reader users can still reach it, but assistive technology will not announce it.");
            }

            if (fragment.TabIndex is > 0)
            {
                yield return Fail(
                    fragment,
                    $"This <{fragment.TagName}> has a positive tabindex ({fragment.TabIndex}), which overrides the natural focus order and can confuse keyboard users. Use tabindex=\"0\" or rely on document order instead.");
            }

            if (!string.IsNullOrEmpty(fragment.AccessKey))
            {
                if (seenAccessKeys.TryGetValue(fragment.AccessKey, out _))
                {
                    yield return Fail(
                        fragment,
                        $"This <{fragment.TagName}> has an accesskey (\"{fragment.AccessKey}\") that duplicates another element on the page, which makes the shortcut unreliable.");
                }
                else
                {
                    seenAccessKeys[fragment.AccessKey] = fragment;
                }
            }
        }
    }

    private Finding Fail(InteractiveAttributesFragment fragment, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        fragment.Location,
        message);
}

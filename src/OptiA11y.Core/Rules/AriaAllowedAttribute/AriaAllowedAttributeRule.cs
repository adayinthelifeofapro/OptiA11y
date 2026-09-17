using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AriaAllowedAttribute;

/// <summary>
/// Flags a widget-state attribute (<c>aria-checked</c>, <c>aria-selected</c>, <c>aria-expanded</c>,
/// <c>aria-pressed</c>) applied to an element whose role doesn't support it (e.g.
/// <c>aria-checked</c> on a <c>role="button"</c>). Scoped to this small, well-established subset
/// of the ARIA-in-HTML attribute-allowance rules rather than the full specification. Role/attribute
/// compatibility within this subset is a fixed spec fact, so this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class AriaAllowedAttributeRule : IContentRule
{
    public string RuleId => "aria-allowed-attribute";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            foreach (var attribute in fragment.DisallowedWidgetStateAttributes)
            {
                var roleDescription = fragment.Role is not null ? $"role=\"{fragment.Role}\"" : "its implicit role";
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Major,
                    Confidence.Fail,
                    fragment.Location,
                    $"This <{fragment.TagName}> has the \"{attribute}\" attribute, which isn't supported on an element with {roleDescription}. Assistive technology will ignore it.");
            }
        }
    }
}

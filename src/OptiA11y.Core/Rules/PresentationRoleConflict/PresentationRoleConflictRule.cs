using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PresentationRoleConflict;

/// <summary>
/// Flags <c>role="presentation"</c>/<c>role="none"</c> applied to an element that is natively
/// focusable, or that also carries a global ARIA attribute (e.g. <c>aria-label</c>). Per the ARIA
/// specification, presentation/none only removes an element's semantics when it is neither
/// focusable nor exposes global ARIA states - browsers ignore the role entirely in those
/// conflicting cases, so this is a deterministic structural fact and reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class PresentationRoleConflictRule : IContentRule
{
    public string RuleId => "presentation-role-conflict";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            var isPresentationRole = string.Equals(fragment.Role, "presentation", StringComparison.OrdinalIgnoreCase)
                || string.Equals(fragment.Role, "none", StringComparison.OrdinalIgnoreCase);

            if (!isPresentationRole)
            {
                continue;
            }

            if (fragment.IsFocusable)
            {
                yield return Fail(fragment, $"This <{fragment.TagName}> has role=\"{fragment.Role}\" but is natively focusable. Per the ARIA spec, the presentation/none role is ignored on focusable elements, so its native semantics remain exposed instead.");
            }

            if (fragment.HasGlobalAriaAttribute)
            {
                yield return Fail(fragment, $"This <{fragment.TagName}> has role=\"{fragment.Role}\" alongside a global aria-* attribute. Per the ARIA spec, the presentation/none role is ignored when global ARIA states/properties are present, so its native semantics remain exposed instead.");
            }
        }
    }

    private Finding Fail(AriaSemanticsFragment fragment, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.Fail,
        fragment.Location,
        message);
}

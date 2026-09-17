using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AriaRequiredChildren;

/// <summary>
/// Flags an element whose ARIA role requires specific owned elements (e.g. <c>role="list"</c>
/// requires <c>role="listitem"</c> descendants, <c>role="tablist"</c> requires <c>role="tab"</c>)
/// but has none. The ARIA specification's required-owned-elements table is a fixed vocabulary
/// (scoped here to a small, well-established subset), so membership is a structural fact and
/// this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class AriaRequiredChildrenRule : IContentRule
{
    public string RuleId => "aria-required-children";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            if (fragment.MissingRequiredOwnedElementDescription is null)
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
                $"This <{fragment.TagName}> has role=\"{fragment.Role}\", which requires owned elements with role=\"{fragment.MissingRequiredOwnedElementDescription}\", but none were found. Assistive technology may not present this structure correctly.");
        }
    }
}

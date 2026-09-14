using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.InvalidAria;

/// <summary>
/// Flags a <c>role</c> value or <c>aria-*</c> attribute name that isn't part of the ARIA
/// specification (almost always a typo, e.g. "aria-lable"). ARIA is a fixed vocabulary, so
/// membership in it is a deterministic fact, not a judgement call - this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class InvalidAriaRule : IContentRule
{
    public string RuleId => "invalid-aria";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaAttributesFragment>())
        {
            if (fragment.RoleIsUnknown)
            {
                yield return Fail(fragment, $"This <{fragment.TagName}> has role=\"{fragment.Role}\", which is not a valid ARIA role. Assistive technology will ignore it, leaving the element with its default (or no) role.");
            }

            foreach (var attributeName in fragment.UnknownAriaAttributeNames)
            {
                yield return Fail(fragment, $"This <{fragment.TagName}> has an attribute \"{attributeName}\" that looks like ARIA but isn't a recognized aria-* attribute - likely a typo. Assistive technology will ignore it.");
            }
        }
    }

    private Finding Fail(AriaAttributesFragment fragment, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        fragment.Location,
        message);
}

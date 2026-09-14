using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.NestedInteractive;

/// <summary>
/// Flags a natively interactive element (link, button, or form control) nested inside another
/// natively interactive ancestor (e.g. a button inside a link) - an invalid interaction model
/// regardless of intent: activating the outer control and the inner one both compete for the
/// same click/tap, and screen readers announce the nesting inconsistently across browsers. This
/// is a structural fact about the markup, so it reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class NestedInteractiveRule : IContentRule
{
    public string RuleId => "nested-interactive";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<InteractiveAttributesFragment>())
        {
            if (!fragment.IsNestedInInteractiveAncestor)
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
                $"This <{fragment.TagName}> is nested inside another interactive element (a link, button, or form control). Nested interactive controls create ambiguous activation behavior and are announced inconsistently by assistive technology.");
        }
    }
}

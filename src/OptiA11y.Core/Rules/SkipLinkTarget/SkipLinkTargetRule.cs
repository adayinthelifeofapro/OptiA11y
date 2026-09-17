using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SkipLinkTarget;

/// <summary>
/// Flags an in-page navigation link (<c>href="#id"</c>) whose target id could not be found
/// within the same parsed HTML fragment. This is scoped to a single property's markup, so an
/// unresolved target is a signal, not proof - the target could legitimately live in the page
/// template or a sibling property this rule cannot see. That ambiguity means this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class SkipLinkTargetRule : IContentRule
{
    public string RuleId => "skip-link-target";

    public string SuccessCriterion => "2.4.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var skipLink in document.Get<SkipLinkFragment>())
        {
            if (skipLink.ResolvedWithinSameFragment)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                skipLink.Location,
                $"This link points at \"#{skipLink.TargetId}\", but no element with that id was found within this property's markup. If \"{skipLink.TargetId}\" isn't defined elsewhere on the rendered page, this link goes nowhere.");
        }
    }
}

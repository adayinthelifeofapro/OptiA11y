using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.BrokenAriaReference;

/// <summary>
/// Flags an <c>aria-labelledby</c>, <c>aria-describedby</c>, or &lt;label for&gt; reference whose
/// target id could not be found within the same parsed HTML fragment. This is scoped to a single
/// property's markup, so an unresolved reference is a signal, not proof - the target could
/// legitimately live in the page template or a sibling property that this rule cannot see. That
/// ambiguity means this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class BrokenAriaReferenceRule : IContentRule
{
    public string RuleId => "broken-aria-reference";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var reference in document.Get<AriaReferenceFragment>())
        {
            if (reference.ResolvedWithinSameFragment)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                reference.Location,
                $"{reference.SourceAttribute}=\"{reference.ReferencedId}\" doesn't resolve to any element within this property's markup. If \"{reference.ReferencedId}\" isn't defined elsewhere on the rendered page, this reference is broken.");
        }
    }
}

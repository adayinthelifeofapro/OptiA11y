using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.StickyObstruction;

/// <summary>
/// Flags a page where sticky/fixed elements consume more than 20% of the small-viewport height
/// (WCAG 1.4.10). <see cref="StickyObstructionFragment"/> is only ever produced by the
/// rendered-style enrichment slice. Whether this specific proportion is actually obstructive for
/// a given layout is a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class StickyObstructionRule : IContentRule
{
    public string RuleId => "sticky-obstruction";

    public string SuccessCriterion => "1.4.10";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var sticky in document.Get<StickyObstructionFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                sticky.Location,
                "Sticky or fixed elements (headers/footers) consume more than a fifth of the viewport height at a narrow (320px) width. Confirm they don't obstruct content that users need to read or interact with.");
        }
    }
}

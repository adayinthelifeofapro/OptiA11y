using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.Reflow;

/// <summary>
/// Flags a page that required horizontal scrolling at a 320px-equivalent viewport width (WCAG
/// 1.4.10). <see cref="ReflowFragment"/> is only ever produced by the rendered-style enrichment
/// slice (<c>OptiA11y.Rendering</c>). 1.4.10 exempts content for which two-dimensional layout is
/// genuinely essential (data tables, images, maps), which this rule has no way to distinguish
/// from a real reflow failure, so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class ReflowRule : IContentRule
{
    public string RuleId => "reflow";

    public string SuccessCriterion => "1.4.10";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var reflow in document.Get<ReflowFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.NeedsReview,
                reflow.Location,
                "This page required horizontal scrolling at a 320px-wide viewport. Confirm the content that overflows (a data table, image, or map) genuinely needs two-dimensional layout - if not, it should reflow to a single column at narrow widths.");
        }
    }
}

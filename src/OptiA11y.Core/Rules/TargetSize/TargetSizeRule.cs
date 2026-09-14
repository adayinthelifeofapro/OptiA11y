using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TargetSize;

/// <summary>
/// Flags an interactive element with a rendered bounding box smaller than 24x24 CSS pixels
/// (WCAG 2.5.8). <see cref="TargetSizeFragment"/> is only ever produced by the rendered-style
/// enrichment slice (<c>OptiA11y.Rendering</c>), which already excludes inline text links and
/// native checkbox/radio inputs (both carved out by 2.5.8's own exceptions) before measuring, so
/// what reaches this rule is a real measured box - a structural fact, hence
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class TargetSizeRule : IContentRule
{
    private const double MinimumSizePx = 24;

    public string RuleId => "target-size";

    public string SuccessCriterion => "2.5.8";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var target in document.Get<TargetSizeFragment>())
        {
            if (target.WidthPx >= MinimumSizePx && target.HeightPx >= MinimumSizePx)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.Fail,
                target.Location,
                $"{target.ElementDescription} renders at {target.WidthPx:F0}x{target.HeightPx:F0}px, below the {MinimumSizePx:F0}x{MinimumSizePx:F0}px minimum target size. Users with limited dexterity may find it hard to activate accurately.");
        }
    }
}

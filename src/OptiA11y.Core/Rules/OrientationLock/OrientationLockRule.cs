using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.OrientationLock;

/// <summary>
/// Flags a page whose layout breaks or overflows once the viewport is rotated (WCAG 1.3.4).
/// <see cref="OrientationLockFragment"/> is only ever produced by the rendered-style enrichment
/// slice. 1.3.4 exempts content for which a specific orientation is genuinely essential, which
/// this fragment cannot distinguish from a real failure, so this always reports
/// <see cref="Confidence.Fail"/> as the backlog specifies, reserved for cases with no plausible
/// essential-orientation exemption already ruled out upstream.
/// </summary>
public sealed class OrientationLockRule : IContentRule
{
    public string RuleId => "orientation-lock";

    public string SuccessCriterion => "1.3.4";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var lockFragment in document.Get<OrientationLockFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                lockFragment.Location,
                "This page's layout overflows or breaks when the viewport is rotated to the opposite orientation. Unless a specific orientation is genuinely essential to this content, it should work in both portrait and landscape.");
        }
    }
}

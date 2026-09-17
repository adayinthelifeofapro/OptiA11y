using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.BypassBlocks;

/// <summary>
/// Flags a page with no skip link, landmark region, or heading that lets users bypass repeated
/// navigation blocks before the main content (WCAG 2.4.1). <see cref="BypassBlocksFragment"/> is
/// only ever produced by the rendered-style enrichment slice from a direct absence check across
/// all three mechanisms, so this is a structural fact, hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class BypassBlocksRule : IContentRule
{
    public string RuleId => "bypass-blocks";

    public string SuccessCriterion => "2.4.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var bypass in document.Get<BypassBlocksFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                bypass.Location,
                "This page has repeated navigation (a nav or header) but no skip link, landmark, or heading before it. Keyboard and screen reader users have no way to bypass the repeated block to reach the main content directly.");
        }
    }
}

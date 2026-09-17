using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ResizeText;

/// <summary>
/// Flags a page that loses or clips content after simulating a 200% text-zoom equivalent (WCAG
/// 1.4.4). <see cref="ResizeTextFragment"/> is only ever produced by the rendered-style
/// enrichment slice from a direct layout-overflow measurement, so this is a structural fact,
/// hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class ResizeTextRule : IContentRule
{
    public string RuleId => "resize-text";

    public string SuccessCriterion => "1.4.4";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var resize in document.Get<ResizeTextFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                resize.Location,
                "This page loses or clips content when text is zoomed to roughly 200%. Users who need larger text should still be able to read and use everything without loss of content or functionality.");
        }
    }
}

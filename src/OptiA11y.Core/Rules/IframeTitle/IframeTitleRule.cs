using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.IframeTitle;

/// <summary>
/// Evaluates iframes for an accessible name. Whether an iframe has a title, aria-label, or
/// aria-labelledby is a deterministic structural fact, so this rule reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class IframeTitleRule : IContentRule
{
    public string RuleId => "iframe-title";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var iframe in document.Get<IframeFragment>())
        {
            if (!iframe.HasAccessibleName)
            {
                yield return Fail(iframe, "This iframe has no title, aria-label, or aria-labelledby. Screen reader users won't know what content it contains.");
            }
        }
    }

    private Finding Fail(IframeFragment iframe, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        iframe.Location,
        message);
}

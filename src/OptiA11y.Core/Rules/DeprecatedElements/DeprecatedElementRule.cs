using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DeprecatedElements;

/// <summary>
/// Flags obsolete/presentational HTML elements (e.g. &lt;blink&gt;, &lt;marquee&gt;, &lt;font&gt;,
/// &lt;center&gt;) that are either inaccessible outright (blink/marquee) or bypass CSS-driven,
/// accessible styling. Presence of the element is a deterministic structural fact, so this rule
/// reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class DeprecatedElementRule : IContentRule
{
    public string RuleId => "deprecated-elements";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<DeprecatedElementFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                element.Location,
                $"A deprecated <{element.TagName}> element was found. This element is obsolete and can cause unpredictable behaviour for assistive technology; replace it with semantic markup and CSS.");
        }
    }
}

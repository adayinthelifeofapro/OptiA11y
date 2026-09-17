using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.BlinkingContent;

/// <summary>
/// Flags an element that blinks via the deprecated &lt;blink&gt;/&lt;marquee&gt; tags or an
/// inline style declaring <c>text-decoration: blink</c> or a <c>blink</c>-named animation, with
/// no way for the user to stop it (WCAG 2.2.2). Detecting the declaration itself is a
/// deterministic structural fact, so this always reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class BlinkingContentRule : IContentRule
{
    public string RuleId => "blinking-content";

    public string SuccessCriterion => "2.2.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<BlinkingContentFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.Fail,
                element.Location,
                $"{element.ElementDescription} blinks automatically with no way to pause, stop, or hide it. Remove the blink effect or provide a control to stop it.");
        }
    }
}

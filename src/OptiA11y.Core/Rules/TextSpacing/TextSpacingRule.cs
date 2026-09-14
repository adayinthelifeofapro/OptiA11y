using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TextSpacing;

/// <summary>
/// Flags text that visually clips after applying the WCAG 1.4.12 reference text-spacing
/// overrides (line height, paragraph spacing, letter spacing, word spacing).
/// <see cref="TextSpacingFragment"/> is only ever produced by the rendered-style enrichment
/// slice (<c>OptiA11y.Rendering</c>), which measures this directly - a real geometric fact with
/// no listed exception in 1.4.12, so this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class TextSpacingRule : IContentRule
{
    public string RuleId => "text-spacing";

    public string SuccessCriterion => "1.4.12";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<TextSpacingFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                fragment.Location,
                $"The text \"{fragment.SampleText}\" is visually clipped once WCAG's reference text-spacing overrides are applied (increased line height, paragraph spacing, letter spacing, and word spacing). Its container needs to grow with the text rather than clip it.");
        }
    }
}

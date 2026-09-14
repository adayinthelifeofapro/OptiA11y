using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SvgAccessibleName;

/// <summary>
/// Flags inline SVG, image-map &lt;area&gt; elements, and &lt;object&gt;/&lt;embed&gt; embeds
/// that expose no accessible name and aren't explicitly marked decorative. Presence of a naming
/// mechanism is a structural fact - like <see cref="OptiA11y.Core.Rules.AltTextQuality.AltTextQualityRule"/>'s
/// missing-alt check, this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class SvgAccessibleNameRule : IContentRule
{
    public string RuleId => "svg-accessible-name";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<NonTextElementFragment>())
        {
            if (element.HasAccessibleName || element.IsAriaHidden)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.Fail,
                element.Location,
                $"This <{element.ElementType}> has no accessible name and isn't marked aria-hidden. Add a <title> (for SVG), an alt/aria-label, or mark it aria-hidden=\"true\" if it conveys no information.");
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.BackgroundImageText;

/// <summary>
/// Flags an inline-styled text block with a <c>background-image</c> set directly on it (WCAG
/// 1.4.5 favors real text over images of text, and text overlaid directly on a background image
/// risks insufficient contrast against busy areas of the image). Detecting the style declaration
/// is deterministic, but whether the actual image harms readability at this location is a
/// judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class BackgroundImageTextRule : IContentRule
{
    public string RuleId => "background-image-text";

    public string SuccessCriterion => "1.4.5";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<TextStyleFragment>())
        {
            if (!fragment.HasBackgroundImage)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"Text \"{fragment.SampleText}\" has a background-image set directly on it. Confirm contrast is sufficient against every part of the image behind the text, especially busier areas.");
        }
    }
}

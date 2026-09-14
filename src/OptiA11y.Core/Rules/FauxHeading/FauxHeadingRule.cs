using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FauxHeading;

/// <summary>
/// Flags a short, fully-bold (or noticeably larger-font) paragraph outside of any real heading
/// element - a common rich-text editor pattern for faking a heading. Whether the editor actually
/// intended it as a heading (rather than, say, a genuinely short bold statement) is a judgement
/// call, so this reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FauxHeadingRule : IContentRule
{
    public string RuleId => "faux-heading";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var block in document.Get<EmphasisBlockFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                block.Location,
                $"The text \"{block.SampleText}\" looks like it's being used as a heading (bold or enlarged, and short), but isn't marked up as one. Screen reader users navigating by heading won't find it. Consider using a real heading element instead.");
        }
    }
}

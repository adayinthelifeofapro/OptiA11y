using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.BlockquoteMisuse;

/// <summary>
/// Flags &lt;blockquote&gt; elements with no citation (&lt;cite&gt; child or <c>cite</c> attribute) —
/// a common sign the element is being used purely for its indentation styling rather than to
/// mark an actual quotation. Whether that's really the intent is a judgement call, so this rule
/// reports <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class BlockquoteMisuseRule : IContentRule
{
    public string RuleId => "blockquote-misuse";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var span in document.Get<MarkupSpanFragment>())
        {
            if (span.TagName != "blockquote" || span.HasCitation)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                span.Location,
                "This <blockquote> has no citation. If it's being used purely for its visual indentation rather than to mark an actual quotation, consider a styled <div> or <p> instead so screen readers don't announce content that isn't really quoted.");
        }
    }
}

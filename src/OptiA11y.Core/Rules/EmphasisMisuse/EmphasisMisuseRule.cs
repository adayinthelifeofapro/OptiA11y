using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.EmphasisMisuse;

/// <summary>
/// Flags &lt;b&gt;/&lt;i&gt; elements wrapping long runs of text, which usually indicates
/// presentational bold/italic markup being used where a screen reader would benefit from real
/// semantic emphasis (&lt;strong&gt;/&lt;em&gt;) or a heading instead. Whether long bold/italic text
/// is a problem depends on editorial intent, so this rule reports
/// <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class EmphasisMisuseRule : IContentRule
{
    private const int LongRunMinLength = 120;

    public string RuleId => "emphasis-misuse";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var span in document.Get<MarkupSpanFragment>())
        {
            if (span.TagName is not ("b" or "i") || span.Text.Length < LongRunMinLength)
            {
                continue;
            }

            var tag = span.TagName == "b" ? "<b>" : "<i>";
            var semanticEquivalent = span.TagName == "b" ? "<strong>" : "<em>";

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                span.Location,
                $"A long run of text ({span.Text.Length} characters) is wrapped in {tag}. If this text carries real emphasis or importance, use {semanticEquivalent}; if it's meant to stand out as a heading, use a real heading element instead.");
        }
    }
}

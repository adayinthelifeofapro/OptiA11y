using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SuperscriptSubscriptMisuse;

/// <summary>
/// Flags &lt;sup&gt;/&lt;sub&gt; elements wrapping long runs of text. These elements are meant for
/// short notational content (footnote markers, chemical formulas, ordinal suffixes); some screen
/// readers announce their content letter-by-letter or with extra verbosity, so wrapping whole
/// sentences in them degrades the reading experience. Whether a given run is "too long" is a
/// judgement call, so this rule reports <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class SuperscriptSubscriptMisuseRule : IContentRule
{
    private const int LongRunMinLength = 20;

    public string RuleId => "superscript-subscript-misuse";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var span in document.Get<MarkupSpanFragment>())
        {
            if (span.TagName is not ("sup" or "sub") || span.Text.Length < LongRunMinLength)
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
                $"A long run of text ({span.Text.Length} characters) is wrapped in <{span.TagName}>. These elements are meant for short notational content (footnote markers, formulas); wrapping full sentences in them can make some screen readers announce the text awkwardly.");
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LineBreakMisuse;

/// <summary>
/// Flags consecutive <c>&lt;br&gt;</c> elements used to fake paragraph spacing instead of real
/// <c>&lt;p&gt;</c> elements. Assistive technology exposes paragraphs as distinct structural
/// units; a run of manual line breaks collapses into a single unstructured block, which harms
/// navigation-by-paragraph for screen reader users. Whether a given run is truly meant as
/// paragraph spacing (versus deliberate visual spacing within a single paragraph) is a judgement
/// call, so this reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LineBreakMisuseRule : IContentRule
{
    public string RuleId => "line-break-misuse";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<LineBreakRunFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"This content has {fragment.ConsecutiveBreakCount} consecutive <br> elements, which looks like it's being used to fake paragraph spacing. Consider using separate <p> elements instead so assistive technology can navigate between paragraphs.");
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TableHeaders;

/// <summary>
/// Evaluates data tables for header cells and a caption/accessible name. Whether a table has
/// header cells at all is a deterministic structural fact, so it may report
/// <see cref="Confidence.Fail"/>; whether a caption is "needed" depends on editorial context
/// (e.g. a single small table might be adequately described by surrounding text), so the
/// caption check is a <see cref="Confidence.NeedsReview"/> heuristic.
/// </summary>
public sealed class TableHeaderRule : IContentRule
{
    public string RuleId => "table-headers";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var table in document.Get<TableFragment>())
        {
            if (table.ColumnCount > 0 && !table.HasHeaderRow)
            {
                yield return Fail(table, "This table has no header cells (<th>). Screen reader users rely on header cells to understand what each cell means.");
            }

            if (!table.HasCaption)
            {
                yield return NeedsReview(table, "This table has no caption or accessible name. Consider adding one so screen reader users know what the table represents.");
            }
        }
    }

    private Finding Fail(TableFragment table, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        table.Location,
        message);

    private Finding NeedsReview(TableFragment table, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.NeedsReview,
        table.Location,
        message);
}

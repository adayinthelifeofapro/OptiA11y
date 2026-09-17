using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LayoutTable;

/// <summary>
/// Flags a table with no header cells and no caption — the combination that most strongly
/// suggests the table is being used for visual layout rather than tabular data. A single missing
/// signal is already covered by <c>table-headers</c>; this rule exists for the layout-table
/// pattern specifically, which is a judgement call about intent, so it is
/// <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class LayoutTableRule : IContentRule
{
    public string RuleId => "layout-table";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var table in document.Get<TableFragment>())
        {
            if (!table.HasHeaderRow && !table.HasCaption && table.ColumnCount > 1)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    table.Location,
                    "This table has no header cells and no caption, which is a common sign it's being used for visual layout rather than tabular data. If it's genuine data, add headers and a caption; if it's layout, consider replacing it with non-table markup.");
            }
        }
    }
}

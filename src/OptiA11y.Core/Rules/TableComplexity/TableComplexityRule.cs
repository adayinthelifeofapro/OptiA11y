using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TableComplexity;

/// <summary>
/// Flags structural table defects beyond the basic header-cell check in
/// <see cref="OptiA11y.Core.Rules.TableHeaders.TableHeaderRule"/>: merged cells (rowspan/colspan)
/// without scope on the header cells that would disambiguate them, and rows with inconsistent
/// cell counts where no merged cells explain the difference. Both are structural facts about the
/// markup, not editorial judgements, so this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class TableComplexityRule : IContentRule
{
    public string RuleId => "table-complexity";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var table in document.Get<TableFragment>())
        {
            if (table.HasMergedCells && !table.AllHeaderCellsHaveScope)
            {
                yield return Fail(
                    table,
                    "This table has merged cells (rowspan/colspan) but not every header cell declares a scope. With merged cells, screen readers can't reliably work out which header applies to which cell without one.");
            }

            if (!table.HasMergedCells && !table.RowLengthsConsistent)
            {
                yield return Fail(
                    table,
                    "This table's rows don't all have the same number of cells, and none of them use rowspan/colspan to explain the difference. This usually means the table markup is malformed.");
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
}

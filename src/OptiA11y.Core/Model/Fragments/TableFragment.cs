namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this table was found.</param>
/// <param name="HasHeaderRow">True when the first row is marked up as header cells.</param>
/// <param name="HasCaption">True when the table has an associated caption or accessible summary.</param>
/// <param name="ColumnCount">The number of columns, used for header-per-column checks.</param>
/// <param name="HasMergedCells">True when any cell uses rowspan or colspan greater than 1.</param>
/// <param name="AllHeaderCellsHaveScope">True when every &lt;th&gt; in the table declares a scope attribute (or there are no header cells at all).</param>
/// <param name="RowLengthsConsistent">True when every row has the same number of cells (accounting for merged cells would require full grid resolution, so this is only meaningful when <see cref="HasMergedCells"/> is false).</param>
public sealed record TableFragment(
    SourceLocation Location,
    bool HasHeaderRow,
    bool HasCaption,
    int ColumnCount,
    bool HasMergedCells = false,
    bool AllHeaderCellsHaveScope = true,
    bool RowLengthsConsistent = true) : ContentFragment(Location);

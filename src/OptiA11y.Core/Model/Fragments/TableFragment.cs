namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this table was found.</param>
/// <param name="HasHeaderRow">True when the first row is marked up as header cells.</param>
/// <param name="HasCaption">True when the table has an associated caption or accessible summary.</param>
/// <param name="ColumnCount">The number of columns, used for header-per-column checks.</param>
public sealed record TableFragment(
    SourceLocation Location,
    bool HasHeaderRow,
    bool HasCaption,
    int ColumnCount) : ContentFragment(Location);

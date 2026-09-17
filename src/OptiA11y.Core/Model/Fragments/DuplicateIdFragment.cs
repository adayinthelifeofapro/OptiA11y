namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where the second (and any subsequent) occurrence of the id was found.</param>
/// <param name="Id">The repeated id value.</param>
/// <param name="OccurrenceCount">How many elements in this property share this id.</param>
public sealed record DuplicateIdFragment(
    SourceLocation Location,
    string Id,
    int OccurrenceCount) : ContentFragment(Location);

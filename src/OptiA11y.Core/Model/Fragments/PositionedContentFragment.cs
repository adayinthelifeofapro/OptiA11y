namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this positioned element was found.</param>
/// <param name="Position">The resolved CSS <c>position</c> value (e.g. "absolute", "fixed").</param>
/// <param name="Float">The resolved CSS <c>float</c> value (e.g. "left", "right"), or null if not set.</param>
/// <param name="SampleText">A short excerpt of the element's visible text, for editorial context.</param>
public sealed record PositionedContentFragment(
    SourceLocation Location,
    string Position,
    string? Float,
    string SampleText) : ContentFragment(Location);

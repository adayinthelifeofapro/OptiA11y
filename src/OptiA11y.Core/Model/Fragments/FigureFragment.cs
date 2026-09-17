namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this figure was found.</param>
/// <param name="AltText">The alt attribute value of the image inside the figure, or null if none is present.</param>
/// <param name="Caption">The trimmed text of the figure's <c>&lt;figcaption&gt;</c>, or null if the figure has none.</param>
public sealed record FigureFragment(
    SourceLocation Location,
    string? AltText,
    string? Caption) : ContentFragment(Location);

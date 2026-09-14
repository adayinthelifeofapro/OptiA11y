namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this styled text was found.</param>
/// <param name="TextAlign">The resolved text-align value, if explicitly set (e.g. "justify").</param>
/// <param name="FontSizePx">The resolved font size in pixels, if explicitly set and parseable.</param>
/// <param name="SampleText">A short excerpt of the affected text, for editor-facing messages.</param>
public sealed record TextStyleFragment(
    SourceLocation Location,
    string? TextAlign,
    double? FontSizePx,
    string SampleText) : ContentFragment(Location);

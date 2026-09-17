namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this styled text was found.</param>
/// <param name="TextAlign">The resolved text-align value, if explicitly set (e.g. "justify").</param>
/// <param name="FontSizePx">The resolved font size in pixels, if explicitly set and parseable.</param>
/// <param name="SampleText">A short excerpt of the affected text, for editor-facing messages.</param>
/// <param name="WidthPx">The resolved inline width in pixels, if explicitly set - used to flag very long line lengths (WCAG 1.4.8, AAA).</param>
/// <param name="LineHeight">The resolved unitless line-height, if explicitly set - used to flag insufficient line spacing (WCAG 1.4.8, AAA).</param>
/// <param name="HasBackgroundImage">True when an inline <c>background-image</c> is set on a text-bearing element, used to flag potential text-over-image readability issues (WCAG 1.4.5).</param>
public sealed record TextStyleFragment(
    SourceLocation Location,
    string? TextAlign,
    double? FontSizePx,
    string SampleText,
    double? WidthPx = null,
    double? LineHeight = null,
    bool HasBackgroundImage = false) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this styled text was found.</param>
/// <param name="ForegroundColor">The resolved foreground (text) color, as authored (hex or rgb).</param>
/// <param name="BackgroundColor">The resolved background color, as authored (hex or rgb).</param>
/// <param name="ContrastRatio">The computed contrast ratio between foreground and background, per WCAG's relative luminance formula.</param>
/// <param name="IsLargeText">True when the text is large enough (per WCAG 1.4.3) to use the reduced 3:1 threshold instead of 4.5:1.</param>
/// <param name="SampleText">A short excerpt of the affected text, for editor-facing messages.</param>
public sealed record ColorContrastFragment(
    SourceLocation Location,
    string ForegroundColor,
    string BackgroundColor,
    double ContrastRatio,
    bool IsLargeText,
    string SampleText) : ContentFragment(Location);

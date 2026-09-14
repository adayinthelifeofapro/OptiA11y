namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this styled text was found.</param>
/// <param name="ForegroundColor">The resolved foreground (text) color, as authored (hex or rgb).</param>
/// <param name="BackgroundColor">The resolved background color, as authored (hex or rgb).</param>
/// <param name="ContrastRatio">The computed contrast ratio between foreground and background, per WCAG's relative luminance formula.</param>
/// <param name="IsLargeText">True when the text is large enough (per WCAG 1.4.3) to use the reduced 3:1 threshold instead of 4.5:1.</param>
/// <param name="SampleText">A short excerpt of the affected text, for editor-facing messages.</param>
/// <param name="BackgroundIsImage">
/// True when the effective background is an image or gradient rather than a flat color - only
/// ever set by the rendered-style enrichment path, since inline-style parsing has no way to know
/// this. The sampled <see cref="BackgroundColor"/> in that case is unreliable (it is whatever
/// solid background-color happens to sit behind the image, not what's visible), so a computed
/// ratio below threshold is not trustworthy enough to Fail on.
/// </param>
public sealed record ColorContrastFragment(
    SourceLocation Location,
    string ForegroundColor,
    string BackgroundColor,
    double ContrastRatio,
    bool IsLargeText,
    string SampleText,
    bool BackgroundIsImage = false) : ContentFragment(Location);

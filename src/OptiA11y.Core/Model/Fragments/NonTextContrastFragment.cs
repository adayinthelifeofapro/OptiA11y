namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// A non-text element (e.g. an inline-styled border or icon) with an explicitly authored color
/// against its effective background, from the inline-style enrichment slice, used to check the
/// 3:1 non-text contrast minimum (WCAG 1.4.11). <see cref="BackgroundColor"/> is the nearest
/// ancestor's explicit <c>background-color</c>; when no ancestor sets one, this defaults to
/// white (<c>#ffffff</c>) as an explicit, documented assumption - real-world default page
/// backgrounds are white far more often than not, but this is not guaranteed, so a marginal
/// ratio close to the 3:1 threshold should still be manually confirmed.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ForegroundColor">The resolved border/icon color, as authored (hex or rgb).</param>
/// <param name="BackgroundColor">The resolved effective background color, as authored (hex or rgb), defaulting to white when no ancestor sets one explicitly.</param>
/// <param name="ContrastRatio">The computed contrast ratio between foreground and background, per WCAG's relative luminance formula.</param>
/// <param name="ElementDescription">A short human-readable description of the element, for editor-facing messages.</param>
public sealed record NonTextContrastFragment(
    SourceLocation Location,
    string ForegroundColor,
    string BackgroundColor,
    double ContrastRatio,
    string ElementDescription) : ContentFragment(Location);

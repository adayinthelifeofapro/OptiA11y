namespace OptiA11y.Rendering;

/// <summary>
/// Everything captured from rendering one content item's preview page in headless Chromium: text
/// styling (for contrast/readability), per-element diagnostics (for target-size/focus-indicator),
/// elements with automatic infinite animation (for motion), whether the page reflows cleanly at
/// a 320px-equivalent viewport, and which text samples clip once the WCAG 1.4.12 reference
/// text-spacing overrides are applied.
/// </summary>
/// <param name="TextStyles">Computed style per visible text node, at the default viewport.</param>
/// <param name="Elements">Bounding box and focus-indicator measurements per interactive element.</param>
/// <param name="AnimatedElementDescriptions">Short descriptions of elements with a CSS animation that repeats indefinitely and isn't paused.</param>
/// <param name="OverflowsAtNarrowViewport">True when the page required horizontal scrolling at a 320px-equivalent viewport width.</param>
/// <param name="TextSpacingClippedSamples">Text samples that were visually clipped after applying the WCAG 1.4.12 reference spacing overrides.</param>
public sealed record RenderedPageDiagnostics(
    IReadOnlyList<RenderedTextStyle> TextStyles,
    IReadOnlyList<RenderedElementDiagnostics> Elements,
    IReadOnlyList<string> AnimatedElementDescriptions,
    bool OverflowsAtNarrowViewport,
    IReadOnlyList<string> TextSpacingClippedSamples)
{
    public static readonly RenderedPageDiagnostics Empty = new(
        Array.Empty<RenderedTextStyle>(),
        Array.Empty<RenderedElementDiagnostics>(),
        Array.Empty<string>(),
        false,
        Array.Empty<string>());

    public bool IsEmpty =>
        TextStyles.Count == 0
        && Elements.Count == 0
        && AnimatedElementDescriptions.Count == 0
        && !OverflowsAtNarrowViewport
        && TextSpacingClippedSamples.Count == 0;
}

namespace OptiA11y.Rendering;

/// <summary>
/// Per-element measurements captured from a rendered page for the target-size and
/// focus-indicator checks. Unlike <see cref="RenderedTextStyle"/> (captured per visible text
/// node), these are captured per interactive element directly, so an icon-only button with no
/// text content is still measured.
/// </summary>
/// <param name="Description">A short human-readable description of the element (tag and accessible name/text), for editor-facing messages.</param>
/// <param name="WidthPx">
/// The rendered bounding box width in CSS pixels, or a large sentinel value when the element is
/// exempt from the WCAG 2.5.8 minimum (an inline text link, or a native checkbox/radio input,
/// both of which the success criterion itself excludes) - the sentinel guarantees it never
/// registers as undersized.
/// </param>
/// <param name="HeightPx">The rendered bounding box height in CSS pixels, subject to the same exemption sentinel as <see cref="WidthPx"/>.</param>
/// <param name="HasVisibleFocusIndicator">
/// True when the element's computed style (outline, box-shadow, background, or border) changes
/// at all between its resting state and when it receives keyboard focus.
/// </param>
public sealed record RenderedElementDiagnostics(
    string Description,
    double WidthPx,
    double HeightPx,
    bool HasVisibleFocusIndicator);

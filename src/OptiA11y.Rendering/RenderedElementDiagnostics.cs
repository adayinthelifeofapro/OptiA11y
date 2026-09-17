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
/// <param name="HasClickHandlerWithoutKeyboardAccess">
/// True when the element has a click handler (or is a common clickable pattern) but is neither
/// a natively focusable/operable element nor exposes a <c>tabindex</c>, so keyboard users cannot
/// reach or activate it (WCAG 2.1.1).
/// </param>
/// <param name="VisibleLabelText">The element's trimmed visible text label, if any (for WCAG 2.5.3 label-in-name).</param>
/// <param name="AccessibleName">The element's computed accessible name, if any (for WCAG 2.5.3 label-in-name).</param>
/// <param name="IsDraggableWithoutAlternative">
/// True when the element exposes drag behaviour (native <c>draggable</c> or pointer-drag
/// handlers) with no discoverable single-pointer alternative such as a button (WCAG 2.5.7).
/// </param>
/// <param name="RequiresMultipointOrPathGesture">
/// True when the element's interaction handlers imply a multipoint or path-based gesture (e.g.
/// touch handlers for pinch/swipe) with no simpler alternative detected (WCAG 2.5.1).
/// </param>
/// <param name="HasHoverOrFocusContentNotPersistent">
/// True when the element reveals additional content on hover/focus that disappears as soon as
/// the pointer moves away or focus is lost, without the content being hoverable/dismissible
/// (WCAG 1.4.13).
/// </param>
/// <param name="HasInsufficientSpacingToNeighbor">
/// True when this target and its nearest interactive neighbour are both under the 24px minimum
/// and closer together than 24px, so neither the size nor the offset exception applies (WCAG 2.5.8).
/// </param>
/// <param name="IsObscuredWhenFocused">
/// True when, once focused, part of the element's bounding box is covered by a sticky/fixed
/// element (header or footer) at the default viewport (WCAG 2.4.11).
/// </param>
public sealed record RenderedElementDiagnostics(
    string Description,
    double WidthPx,
    double HeightPx,
    bool HasVisibleFocusIndicator,
    bool HasClickHandlerWithoutKeyboardAccess = false,
    string? VisibleLabelText = null,
    string? AccessibleName = null,
    bool IsDraggableWithoutAlternative = false,
    bool RequiresMultipointOrPathGesture = false,
    bool HasHoverOrFocusContentNotPersistent = false,
    bool HasInsufficientSpacingToNeighbor = false,
    bool IsObscuredWhenFocused = false);

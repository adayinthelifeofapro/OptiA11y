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
/// <param name="HasKeyboardTrap">True when sequential Tab presses could not escape a component within a bounded number of iterations (WCAG 2.1.2).</param>
/// <param name="FocusOrderDivergesFromVisualOrder">True when the DOM/keyboard tab order visits elements in a different order than their visual (reading) layout order (WCAG 2.4.3).</param>
/// <param name="FocusAppearanceBelowThreshold">Descriptions of focused elements whose focus indicator area/contrast falls below the AAA appearance thresholds (WCAG 2.4.13).</param>
/// <param name="OverflowsOrLosesContentUnderOrientationLock">True when the page enforces a fixed orientation or breaks layout when the viewport is rotated (WCAG 1.3.4).</param>
/// <param name="LosesContentAtTextZoom">True when content is clipped or lost after simulating 200% text zoom (WCAG 1.4.4).</param>
/// <param name="StickyElementsConsumeExcessiveViewport">True when sticky/fixed elements occupy more than 20% of the small-viewport height, obstructing content (WCAG 1.4.10).</param>
/// <param name="HasThresholdExceedingFlash">True when an element's animation/transition timing implies flashes more frequent than the general flash threshold (WCAG 2.3.1).</param>
/// <param name="DynamicStatusContentWithoutLiveRegion">Descriptions of content inserted dynamically (via script) into an element lacking any live-region semantics (WCAG 4.1.3).</param>
/// <param name="HasBypassMechanism">True when the page provides a skip link, landmark region, or heading that lets users bypass repeated navigation blocks (WCAG 2.4.1).</param>
/// <param name="HasMainLandmark">True when the page has exactly one <c>main</c> landmark (WCAG 1.3.1).</param>
/// <param name="ContentOutsideAnyLandmark">Trimmed text samples of visible content that sits outside every landmark region (WCAG 1.3.1).</param>
/// <param name="DuplicateUnlabelledLandmarkDescriptions">Descriptions of landmark regions that share a role with another landmark and have no distinguishing accessible name (WCAG 1.3.1).</param>
/// <param name="HeadingOrderDivergesFromVisualOrder">True when the DOM heading order differs from the order headings appear visually on screen (WCAG 1.3.2).</param>
public sealed record RenderedPageDiagnostics(
    IReadOnlyList<RenderedTextStyle> TextStyles,
    IReadOnlyList<RenderedElementDiagnostics> Elements,
    IReadOnlyList<string> AnimatedElementDescriptions,
    bool OverflowsAtNarrowViewport,
    IReadOnlyList<string> TextSpacingClippedSamples,
    bool HasKeyboardTrap = false,
    bool FocusOrderDivergesFromVisualOrder = false,
    IReadOnlyList<string>? FocusAppearanceBelowThreshold = null,
    bool OverflowsOrLosesContentUnderOrientationLock = false,
    bool LosesContentAtTextZoom = false,
    bool StickyElementsConsumeExcessiveViewport = false,
    bool HasThresholdExceedingFlash = false,
    IReadOnlyList<string>? DynamicStatusContentWithoutLiveRegion = null,
    bool HasBypassMechanism = true,
    bool HasMainLandmark = true,
    IReadOnlyList<string>? ContentOutsideAnyLandmark = null,
    IReadOnlyList<string>? DuplicateUnlabelledLandmarkDescriptions = null,
    bool HeadingOrderDivergesFromVisualOrder = false)
{
    public static readonly RenderedPageDiagnostics Empty = new(
        Array.Empty<RenderedTextStyle>(),
        Array.Empty<RenderedElementDiagnostics>(),
        Array.Empty<string>(),
        false,
        Array.Empty<string>(),
        FocusAppearanceBelowThreshold: Array.Empty<string>(),
        DynamicStatusContentWithoutLiveRegion: Array.Empty<string>(),
        ContentOutsideAnyLandmark: Array.Empty<string>(),
        DuplicateUnlabelledLandmarkDescriptions: Array.Empty<string>());

    public bool IsEmpty =>
        TextStyles.Count == 0
        && Elements.Count == 0
        && AnimatedElementDescriptions.Count == 0
        && !OverflowsAtNarrowViewport
        && TextSpacingClippedSamples.Count == 0
        && !HasKeyboardTrap
        && !FocusOrderDivergesFromVisualOrder
        && (FocusAppearanceBelowThreshold?.Count ?? 0) == 0
        && !OverflowsOrLosesContentUnderOrientationLock
        && !LosesContentAtTextZoom
        && !StickyElementsConsumeExcessiveViewport
        && !HasThresholdExceedingFlash
        && (DynamicStatusContentWithoutLiveRegion?.Count ?? 0) == 0
        && HasBypassMechanism
        && HasMainLandmark
        && (ContentOutsideAnyLandmark?.Count ?? 0) == 0
        && (DuplicateUnlabelledLandmarkDescriptions?.Count ?? 0) == 0
        && !HeadingOrderDivergesFromVisualOrder;
}

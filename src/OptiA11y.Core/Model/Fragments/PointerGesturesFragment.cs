namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an element whose interaction handlers imply a multipoint or path-based gesture (e.g.
/// touch handlers for pinch/swipe) with no simpler single-pointer alternative detected (WCAG
/// 2.5.1), from the rendered-style enrichment slice. Detecting the handler is structural, but
/// confirming the absence of any alternative elsewhere on the page is not fully verifiable.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record PointerGesturesFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

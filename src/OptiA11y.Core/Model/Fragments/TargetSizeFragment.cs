namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// The rendered bounding box of an interactive element, from the rendered-style enrichment
/// slice (<c>OptiA11y.Rendering</c>) - inline HTML parsing alone cannot measure a real layout
/// box, so this is only ever produced by <c>RenderedStyleFragmentBuilder</c>.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element (tag and accessible name/text), for editor-facing messages.</param>
/// <param name="WidthPx">The rendered width in CSS pixels.</param>
/// <param name="HeightPx">The rendered height in CSS pixels.</param>
public sealed record TargetSizeFragment(
    SourceLocation Location,
    string ElementDescription,
    double WidthPx,
    double HeightPx) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page's layout breaks or overflows once the viewport is rotated
/// (WCAG 1.3.4), from the rendered-style enrichment slice. 1.3.4 exempts content for which a
/// specific orientation is genuinely essential, which this fragment cannot distinguish from a
/// real orientation-lock failure.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record OrientationLockFragment(SourceLocation Location) : ContentFragment(Location);

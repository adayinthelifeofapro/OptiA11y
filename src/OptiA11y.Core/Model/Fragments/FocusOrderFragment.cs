namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page's DOM/keyboard tab order visits focusable elements in a
/// different sequence than their visual (top-to-bottom, left-to-right) layout order (WCAG
/// 2.4.3), from the rendered-style enrichment slice. Whether a given divergence is actually
/// disorienting depends on the page's specific layout, so this is a judgement call.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record FocusOrderFragment(SourceLocation Location) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page's heading order in the DOM diverges from the order headings
/// appear visually on screen (WCAG 1.3.2), from the rendered-style enrichment slice. Whether a
/// given divergence actually confuses assistive-technology users (who typically navigate by DOM
/// heading order) is a judgement call.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record HeadingInViewportOrderFragment(SourceLocation Location) : ContentFragment(Location);

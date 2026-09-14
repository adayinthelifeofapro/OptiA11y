namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page required horizontal scrolling at a 320px-equivalent viewport
/// width, from the rendered-style enrichment slice. WCAG 1.4.10 carves out exceptions (data
/// tables, images, maps, and other content for which 2D layout is essential) that this fragment
/// cannot distinguish from a genuine reflow failure, so the corresponding rule reports
/// NeedsReview rather than Fail.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record ReflowFragment(SourceLocation Location) : ContentFragment(Location);

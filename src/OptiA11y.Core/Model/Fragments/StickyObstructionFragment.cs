namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that sticky/fixed elements consume more than 20% of the small-viewport height,
/// potentially obstructing content underneath (WCAG 1.4.10), from the rendered-style enrichment
/// slice. Whether this specific proportion is actually obstructive for a given layout is a
/// judgement call.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record StickyObstructionFragment(SourceLocation Location) : ContentFragment(Location);

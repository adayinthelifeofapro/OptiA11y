namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page loses or clips content after simulating a 200% text-zoom
/// equivalent (WCAG 1.4.4), from the rendered-style enrichment slice. This is a structurally
/// observed layout failure rather than a heuristic guess.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record ResizeTextFragment(SourceLocation Location) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that the rendered page has no skip link, landmark region, or heading that lets users
/// bypass repeated navigation blocks before the main content (WCAG 2.4.1), from the
/// rendered-style enrichment slice. Absence of all three mechanisms is a directly observable
/// structural fact.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record BypassBlocksFragment(SourceLocation Location) : ContentFragment(Location);

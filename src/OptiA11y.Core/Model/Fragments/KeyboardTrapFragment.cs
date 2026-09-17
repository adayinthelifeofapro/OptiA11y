namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that sequential Tab-key presses could not escape a component within a bounded number
/// of iterations, from the rendered-style enrichment slice (<c>OptiA11y.Rendering</c>) - only
/// a real keyboard simulation against a live page can observe this (WCAG 2.1.2).
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record KeyboardTrapFragment(SourceLocation Location) : ContentFragment(Location);

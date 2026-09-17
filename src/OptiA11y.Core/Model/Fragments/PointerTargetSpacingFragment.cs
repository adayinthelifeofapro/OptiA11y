namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an interactive target that is both under the 24px WCAG 2.5.8 minimum and closer than
/// 24px to its nearest interactive neighbour, so neither the minimum-size nor the
/// sufficient-offset exception applies, from the rendered-style enrichment slice.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record PointerTargetSpacingFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

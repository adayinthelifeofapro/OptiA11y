namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks a focusable element whose focus indicator is estimated to fall below the WCAG 2.4.13
/// (AAA) appearance thresholds (a sufficiently thick, high-contrast outline or equivalent),
/// from the rendered-style enrichment slice. Estimating true perimeter/contrast area against the
/// AAA formula from computed styles alone is inherently approximate.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record FocusAppearanceFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

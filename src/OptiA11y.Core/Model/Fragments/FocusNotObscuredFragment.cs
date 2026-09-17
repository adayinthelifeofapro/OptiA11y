namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks a focusable element whose bounding box is at least partially covered by a sticky/fixed
/// element (header or footer) once it receives keyboard focus (WCAG 2.4.11), from the
/// rendered-style enrichment slice. This is a structural, directly measured fact.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the obscured element.</param>
public sealed record FocusNotObscuredFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

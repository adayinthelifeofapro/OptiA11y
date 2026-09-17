namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an element whose visible label text is not fully contained within its computed
/// accessible name (WCAG 2.5.3), from the rendered-style enrichment slice. A speech-input user
/// who speaks the visible label may fail to activate the control if the accessible name doesn't
/// contain it, which is a directly measurable structural mismatch.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
/// <param name="VisibleLabelText">The element's trimmed visible text label.</param>
/// <param name="AccessibleName">The element's computed accessible name.</param>
public sealed record LabelInNameFragment(
    SourceLocation Location,
    string ElementDescription,
    string VisibleLabelText,
    string AccessibleName) : ContentFragment(Location);

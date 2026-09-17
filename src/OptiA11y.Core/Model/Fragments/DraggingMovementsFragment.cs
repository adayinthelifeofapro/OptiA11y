namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an element that exposes drag behaviour with no discoverable single-pointer alternative
/// such as a button (WCAG 2.5.7), from the rendered-style enrichment slice. Confirming the
/// absence of any alternative elsewhere on the page is not fully verifiable, so this is a
/// judgement call.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record DraggingMovementsFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

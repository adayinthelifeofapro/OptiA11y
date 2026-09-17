namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an element with a click handler that is neither natively focusable/operable nor
/// exposes a <c>tabindex</c>, so keyboard users cannot reach or activate it (WCAG 2.1.1), from
/// the rendered-style enrichment slice. Detecting the presence of a click handler this way is a
/// structural fact about the rendered DOM.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record KeyboardOperableFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

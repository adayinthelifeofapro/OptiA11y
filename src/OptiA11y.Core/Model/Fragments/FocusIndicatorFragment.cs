namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Records whether a focusable element's rendered appearance changes at all when it receives
/// keyboard focus (outline, box-shadow, background, or border), from the rendered-style
/// enrichment slice. Only ever produced by <c>RenderedStyleFragmentBuilder</c>.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element (tag and accessible name/text), for editor-facing messages.</param>
public sealed record FocusIndicatorFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

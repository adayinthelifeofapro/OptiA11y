namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks an element that reveals additional content on hover/focus which disappears as soon as
/// the pointer moves away or focus is lost, without the revealed content being hoverable,
/// dismissible, or persistent (WCAG 1.4.13), from the rendered-style enrichment slice. Whether
/// the specific interaction pattern is actually a problem for users is a judgement call.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record HoverFocusContentFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

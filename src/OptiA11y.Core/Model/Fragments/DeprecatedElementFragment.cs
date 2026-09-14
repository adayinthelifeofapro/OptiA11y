namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The deprecated/obsolete element's tag name, e.g. "blink", "marquee", "font", "center".</param>
public sealed record DeprecatedElementFragment(
    SourceLocation Location,
    string TagName) : ContentFragment(Location);

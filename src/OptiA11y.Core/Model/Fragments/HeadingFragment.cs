namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this heading was found.</param>
/// <param name="Level">The heading level, 1 through 6.</param>
/// <param name="Text">The heading's text content.</param>
public sealed record HeadingFragment(
    SourceLocation Location,
    int Level,
    string Text) : ContentFragment(Location);

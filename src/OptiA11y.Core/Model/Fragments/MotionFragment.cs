namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// An element with a CSS animation that repeats indefinitely and starts automatically, from the
/// rendered-style enrichment slice. Only ever produced by <c>RenderedStyleFragmentBuilder</c>.
/// WCAG 2.2.2 allows an "essential" exception this fragment cannot detect, so the corresponding
/// rule reports NeedsReview rather than Fail.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element, for editor-facing messages.</param>
public sealed record MotionFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that a piece of text was visually clipped (overflow hidden, or its container failed to
/// grow) after applying the WCAG 1.4.12 reference text-spacing overrides, from the rendered-style
/// enrichment slice. Only ever produced by <c>RenderedStyleFragmentBuilder</c>.
/// </summary>
/// <param name="Location">Where this text was found.</param>
/// <param name="SampleText">A short excerpt of the affected text, for editor-facing messages.</param>
public sealed record TextSpacingFragment(
    SourceLocation Location,
    string SampleText) : ContentFragment(Location);

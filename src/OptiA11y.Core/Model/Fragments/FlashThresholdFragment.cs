namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks that an element's animation/transition timing implies flashes more frequent than the
/// WCAG 2.3.1 general flash threshold (roughly 3 times per second), from the rendered-style
/// enrichment slice. This is an animation-timing heuristic, not true sampled luminance analysis
/// (which would require frame-by-frame screenshot comparison), so it always reports
/// <see cref="Confidence.NeedsReview"/> even though the table associates 2.3.1 with Fail-level
/// certainty in the ideal case.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
public sealed record FlashThresholdFragment(SourceLocation Location) : ContentFragment(Location);

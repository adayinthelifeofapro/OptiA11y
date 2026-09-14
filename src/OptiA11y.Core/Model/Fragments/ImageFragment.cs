namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this image was found.</param>
/// <param name="Src">The image source URL or asset reference, used for diagnostics only.</param>
/// <param name="AltText">The alt attribute value, or null if the attribute is entirely absent (distinct from an empty string, which is a deliberate "decorative" signal).</param>
/// <param name="IsDecorativeCandidate">True when the surrounding markup suggests the image may be presentational (e.g. inside a link with other text, or explicitly marked decorative).</param>
public sealed record ImageFragment(
    SourceLocation Location,
    string Src,
    string? AltText,
    bool IsDecorativeCandidate) : ContentFragment(Location);

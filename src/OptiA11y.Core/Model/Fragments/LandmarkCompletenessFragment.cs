namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Captures landmark-region completeness for the rendered page (WCAG 1.3.1), from the
/// rendered-style enrichment slice: whether exactly one <c>main</c> landmark exists, any visible
/// content that sits outside every landmark region, and any landmarks sharing a role with
/// another landmark with no distinguishing accessible name. Whether a given gap is actually
/// disorienting for assistive-technology users depends on the page's specific structure.
/// </summary>
/// <param name="Location">The audited content item's location.</param>
/// <param name="HasMainLandmark">True when the page has exactly one <c>main</c> landmark.</param>
/// <param name="ContentOutsideAnyLandmark">Trimmed text samples of visible content outside every landmark region.</param>
/// <param name="DuplicateUnlabelledLandmarkDescriptions">Descriptions of landmarks sharing a role with another landmark and lacking a distinguishing accessible name.</param>
public sealed record LandmarkCompletenessFragment(
    SourceLocation Location,
    bool HasMainLandmark,
    IReadOnlyList<string> ContentOutsideAnyLandmark,
    IReadOnlyList<string> DuplicateUnlabelledLandmarkDescriptions) : ContentFragment(Location);

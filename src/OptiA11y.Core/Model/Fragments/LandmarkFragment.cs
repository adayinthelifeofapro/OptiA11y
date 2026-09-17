namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Captures a landmark-role element (explicit role or an element with a native landmark role,
/// such as <c>&lt;main&gt;</c> or <c>&lt;nav&gt;</c>) found within a single content property's
/// markup. Used to flag landmark roles embedded inside content markup (which usually belong to
/// the page template instead) and duplicate landmarks with no distinguishing accessible name.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The element's tag name.</param>
/// <param name="Role">The effective landmark role - either explicit via <c>role</c>, or the tag's native implicit landmark role.</param>
/// <param name="IsExplicitRole">True when the role was set via an explicit <c>role</c> attribute rather than implied by the tag name.</param>
/// <param name="HasAccessibleName">True when the element has an <c>aria-label</c> or <c>aria-labelledby</c> distinguishing it from other landmarks of the same role.</param>
public sealed record LandmarkFragment(
    SourceLocation Location,
    string TagName,
    string Role,
    bool IsExplicitRole,
    bool HasAccessibleName) : ContentFragment(Location);

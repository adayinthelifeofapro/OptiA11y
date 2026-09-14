namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Captures ARIA vocabulary usage on an element, for validation against the fixed set of ARIA
/// roles and states/properties. Only emitted for elements that declare a <c>role</c> or at least
/// one <c>aria-*</c> attribute, matching the "noteworthy only" pattern used elsewhere in the
/// parser to keep fragment volume down.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The element's tag name.</param>
/// <param name="Role">The element's <c>role</c> attribute value, or null if absent.</param>
/// <param name="RoleIsUnknown">True when <see cref="Role"/> is set but is not a value from the ARIA specification - this is a fixed vocabulary, so this is a deterministic structural fact, not a judgement call.</param>
/// <param name="UnknownAriaAttributeNames">
/// Any <c>aria-*</c> attribute names present on the element that are not part of the ARIA
/// specification (most often a typo, e.g. "aria-lable"). Deterministic for the same reason as
/// <see cref="RoleIsUnknown"/>.
/// </param>
public sealed record AriaAttributesFragment(
    SourceLocation Location,
    string TagName,
    string? Role,
    bool RoleIsUnknown,
    IReadOnlyList<string> UnknownAriaAttributeNames) : ContentFragment(Location);

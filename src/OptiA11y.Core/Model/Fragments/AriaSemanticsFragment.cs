namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Captures deeper ARIA semantics for an element beyond plain vocabulary validity (handled by
/// <see cref="AriaAttributesFragment"/>): whether its explicit role has required owned elements
/// or required state/property attributes that are missing, whether it carries a widget-state
/// attribute that isn't allowed on its role, whether it is aria-hidden but contains a focusable
/// descendant, whether its role="presentation"/"none" conflicts with being focusable or carrying
/// global ARIA attributes, and whether its explicit role merely duplicates the element's native
/// implicit role. Only emitted when at least one of these signals is present, matching the
/// "noteworthy only" pattern used elsewhere in the parser.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The element's tag name.</param>
/// <param name="Role">The element's explicit <c>role</c> attribute value, or null if absent.</param>
/// <param name="MissingRequiredOwnedElementDescription">
/// A human-readable description of the required owned element type missing from this role's
/// children (e.g. "listitem" for role="list"), or null when not applicable/satisfied.
/// Deterministic: the ARIA spec's required-owned-elements table is fixed.
/// </param>
/// <param name="MissingRequiredAttributes">
/// Any <c>aria-*</c> attributes that this role requires but that are absent from the element.
/// Deterministic: the ARIA spec's required-attributes table is fixed.
/// </param>
/// <param name="DisallowedWidgetStateAttributes">
/// Any widget-state attributes (<c>aria-checked</c>, <c>aria-selected</c>, <c>aria-expanded</c>,
/// <c>aria-pressed</c>) present on the element whose role does not support them. Scoped to this
/// small, well-established subset of the ARIA-in-HTML attribute-allowance rules rather than the
/// full specification.
/// </param>
/// <param name="IsAriaHiddenWithFocusableDescendant">
/// True when this element has <c>aria-hidden="true"</c> and contains at least one natively
/// focusable descendant. Deterministic: assistive technology and the browser disagree in this
/// case, which is always invalid regardless of authoring intent.
/// </param>
/// <param name="IsFocusable">True when the element itself is natively focusable/interactive.</param>
/// <param name="HasGlobalAriaAttribute">True when the element carries at least one global <c>aria-*</c> attribute (e.g. <c>aria-label</c>), used to detect conflicts with <c>role="presentation"</c>/<c>"none"</c>.</param>
/// <param name="IsRedundantRole">
/// True when the element's explicit role is identical to its native implicit role (e.g.
/// <c>&lt;button role="button"&gt;</c>, <c>&lt;nav role="navigation"&gt;</c>). This is a judgement
/// call - some authors add it deliberately for older assistive technology compatibility - so
/// findings based on this are never <see cref="Confidence.Fail"/>.
/// </param>
public sealed record AriaSemanticsFragment(
    SourceLocation Location,
    string TagName,
    string? Role,
    string? MissingRequiredOwnedElementDescription,
    IReadOnlyList<string> MissingRequiredAttributes,
    IReadOnlyList<string> DisallowedWidgetStateAttributes,
    bool IsAriaHiddenWithFocusableDescendant,
    bool IsFocusable,
    bool HasGlobalAriaAttribute,
    bool IsRedundantRole) : ContentFragment(Location);

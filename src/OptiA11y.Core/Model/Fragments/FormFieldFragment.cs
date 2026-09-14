namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this form control was found.</param>
/// <param name="ControlType">The kind of control, e.g. "input", "select", "textarea".</param>
/// <param name="InputType">The <c>type</c> attribute for &lt;input&gt; elements (e.g. "text", "checkbox"), or null for other control types.</param>
/// <param name="HasAccessibleName">
/// True when the control has an accessible name via an associated &lt;label&gt;, <c>aria-label</c>,
/// or <c>aria-labelledby</c>. Deterministic: presence of one of these mechanisms is a structural fact.
/// </param>
/// <param name="AutocompleteToken">The raw <c>autocomplete</c> attribute value, or null if absent.</param>
/// <param name="InferredPurposeCategory">
/// A best-effort guess (from the field's id/name/placeholder/label text) at which WCAG 1.3.5 input
/// purpose this field serves (e.g. "email", "tel", "name"), or null when no known purpose keyword
/// matched. This is a heuristic signal only, never a structural fact.
/// </param>
public sealed record FormFieldFragment(
    SourceLocation Location,
    string ControlType,
    string? InputType,
    bool HasAccessibleName,
    string? AutocompleteToken = null,
    string? InferredPurposeCategory = null) : ContentFragment(Location);

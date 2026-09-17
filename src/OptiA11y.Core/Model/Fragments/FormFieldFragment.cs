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
/// <param name="LabelText">The trimmed text of the field's associated &lt;label&gt;, or null if none could be resolved.</param>
/// <param name="PlaceholderText">The raw <c>placeholder</c> attribute value, or null if absent.</param>
/// <param name="IsRequired">True when the field has a <c>required</c> attribute or <c>aria-required="true"</c>. Deterministic structural fact.</param>
/// <param name="HasVisualRequiredIndicator">True when the field's label or placeholder text contains an asterisk, a common visual convention for marking a field required.</param>
/// <param name="HasAriaDescribedBy">True when the field has an <c>aria-describedby</c> attribute, used to check whether instructions/errors are programmatically associated.</param>
/// <param name="IsDisabled">True when the field has a native <c>disabled</c> attribute.</param>
/// <param name="AriaDisabled">True when the field has <c>aria-disabled="true"</c>.</param>
/// <param name="FirstOptionText">For &lt;select&gt; controls, the trimmed text of the first &lt;option&gt;, or null for other control types or empty selects.</param>
/// <param name="HasEmptyOptionText">For &lt;select&gt; controls, true when any &lt;option&gt; has empty/whitespace-only text.</param>
/// <param name="AriaInvalid">True when the field has <c>aria-invalid="true"</c>, signalling a validation error.</param>
/// <param name="HasPatternOrFormatConstraint">True when the field has a <c>pattern</c> attribute, or an input type (email, tel, url, date, etc.) that implies a specific expected format.</param>
public sealed record FormFieldFragment(
    SourceLocation Location,
    string ControlType,
    string? InputType,
    bool HasAccessibleName,
    string? AutocompleteToken = null,
    string? InferredPurposeCategory = null,
    string? LabelText = null,
    string? PlaceholderText = null,
    bool IsRequired = false,
    bool HasVisualRequiredIndicator = false,
    bool HasAriaDescribedBy = false,
    bool IsDisabled = false,
    bool AriaDisabled = false,
    string? FirstOptionText = null,
    bool HasEmptyOptionText = false,
    bool AriaInvalid = false,
    bool HasPatternOrFormatConstraint = false) : ContentFragment(Location);

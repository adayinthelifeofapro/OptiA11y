namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this form control was found.</param>
/// <param name="ControlType">The kind of control, e.g. "input", "select", "textarea".</param>
/// <param name="InputType">The <c>type</c> attribute for &lt;input&gt; elements (e.g. "text", "checkbox"), or null for other control types.</param>
/// <param name="HasAccessibleName">
/// True when the control has an accessible name via an associated &lt;label&gt;, <c>aria-label</c>,
/// or <c>aria-labelledby</c>. Deterministic: presence of one of these mechanisms is a structural fact.
/// </param>
public sealed record FormFieldFragment(
    SourceLocation Location,
    string ControlType,
    string? InputType,
    bool HasAccessibleName) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this button-like control was found.</param>
/// <param name="ControlType">The element/type that makes this a button, e.g. "button", "input[submit]".</param>
/// <param name="HasAccessibleName">
/// True when the control has an accessible name from its text content, <c>value</c> attribute,
/// <c>aria-label</c>, or <c>aria-labelledby</c>. Deterministic: presence of one of these
/// mechanisms is a structural fact, not a judgement about wording quality.
/// </param>
public sealed record ButtonFragment(
    SourceLocation Location,
    string ControlType,
    bool HasAccessibleName) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this button-like control was found.</param>
/// <param name="ControlType">"button", or "input[submit]" etc. for button-like inputs.</param>
/// <param name="HasAccessibleName">True when the control has visible text, a value attribute (for inputs), or an aria-label.</param>
/// <param name="TitleAttribute">The raw title attribute value, if present, used to flag redundant or unreliable use of title as a naming mechanism.</param>
public sealed record ButtonFragment(
    SourceLocation Location,
    string ControlType,
    bool HasAccessibleName,
    string? TitleAttribute = null) : ContentFragment(Location);

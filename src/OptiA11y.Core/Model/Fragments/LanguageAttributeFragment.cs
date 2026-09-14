namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this language declaration was found.</param>
/// <param name="LanguageCode">The raw value of the <c>lang</c> attribute.</param>
/// <param name="IsWellFormed">True when the value looks like a valid BCP 47 language tag (e.g. "en", "en-GB", "fr-CA").</param>
public sealed record LanguageAttributeFragment(
    SourceLocation Location,
    string LanguageCode,
    bool IsWellFormed) : ContentFragment(Location);

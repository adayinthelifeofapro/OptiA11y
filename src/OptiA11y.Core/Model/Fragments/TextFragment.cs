namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this text block was found.</param>
/// <param name="Text">The plain-text content, used for language-of-parts and readability style checks.</param>
/// <param name="LanguageCode">The declared language for this text, if any (e.g. from a lang attribute or property-level setting).</param>
public sealed record TextFragment(
    SourceLocation Location,
    string Text,
    string? LanguageCode) : ContentFragment(Location);

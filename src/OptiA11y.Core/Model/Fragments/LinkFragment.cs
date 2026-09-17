namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this link was found.</param>
/// <param name="Href">The link target, used for diagnostics and duplicate-text detection.</param>
/// <param name="Text">The visible link text.</param>
/// <param name="IsDocumentLink">True when the href points at a downloadable document (PDF, DOCX, etc.) rather than a page, triggering document-labelling checks.</param>
/// <param name="HasAccessibleName">True when the link has visible text, an aria-label/aria-labelledby, or wraps an image with non-empty alt text. Deterministic - unlike text quality, whether an accessible name exists at all is a structural fact.</param>
/// <param name="TitleAttribute">The raw title attribute value, if present, used to flag redundant or unreliable use of title as a naming mechanism.</param>
/// <param name="Target">The raw target attribute value, if present, used to flag links that open in a new window/tab with no warning.</param>
/// <param name="LanguageCode">The nearest declared `lang` attribute (from the link itself or an ancestor), used to detect link text written in a different script than declared.</param>
/// <param name="HasExplicitColorStyle">True when the link has an inline <c>color</c> style differing from its surrounding text, a heuristic signal it may rely on color alone to be distinguished from body text.</param>
/// <param name="RemovesUnderline">True when the link has an inline <c>text-decoration: none</c> style, removing the other common non-color visual cue.</param>
public sealed record LinkFragment(
    SourceLocation Location,
    string Href,
    string Text,
    bool IsDocumentLink,
    bool HasAccessibleName = true,
    string? TitleAttribute = null,
    string? Target = null,
    string? LanguageCode = null,
    bool HasExplicitColorStyle = false,
    bool RemovesUnderline = false) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this link was found.</param>
/// <param name="Href">The link target, used for diagnostics and duplicate-text detection.</param>
/// <param name="Text">The visible link text.</param>
/// <param name="IsDocumentLink">True when the href points at a downloadable document (PDF, DOCX, etc.) rather than a page, triggering document-labelling checks.</param>
public sealed record LinkFragment(
    SourceLocation Location,
    string Href,
    string Text,
    bool IsDocumentLink) : ContentFragment(Location);

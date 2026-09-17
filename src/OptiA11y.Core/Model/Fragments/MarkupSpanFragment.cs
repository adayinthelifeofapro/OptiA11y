namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this markup span was found.</param>
/// <param name="TagName">The element name, e.g. "blockquote", "sup", "sub", "b", "i".</param>
/// <param name="Text">The element's trimmed visible text.</param>
/// <param name="HasCitation">For blockquote: true when a <c>cite</c> attribute or a <c>&lt;cite&gt;</c> descendant is present.</param>
public sealed record MarkupSpanFragment(
    SourceLocation Location,
    string TagName,
    string Text,
    bool HasCitation = false) : ContentFragment(Location);

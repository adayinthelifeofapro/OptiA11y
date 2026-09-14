namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this iframe was found.</param>
/// <param name="Src">The iframe's source URL, used for diagnostics.</param>
/// <param name="HasAccessibleName">
/// True when the iframe has a <c>title</c>, <c>aria-label</c>, or <c>aria-labelledby</c>
/// attribute. Deterministic: presence of one of these mechanisms is a structural fact.
/// </param>
public sealed record IframeFragment(
    SourceLocation Location,
    string Src,
    bool HasAccessibleName) : ContentFragment(Location);

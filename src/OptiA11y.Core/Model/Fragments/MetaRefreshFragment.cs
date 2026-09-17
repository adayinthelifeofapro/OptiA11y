namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// A <c>&lt;meta http-equiv="refresh"&gt;</c> tag found in content markup. This is a deterministic
/// structural fact - the tag either exists or it doesn't - so the corresponding rule reports
/// <see cref="Model.Confidence.Fail"/>.
/// </summary>
/// <param name="Location">Where this meta tag was found.</param>
/// <param name="Content">The raw content attribute value, e.g. "5" or "5;url=https://example.com".</param>
public sealed record MetaRefreshFragment(
    SourceLocation Location,
    string Content) : ContentFragment(Location);

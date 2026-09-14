namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Content-level metadata that doesn't come from HTML at all - it comes from the content item
/// itself (its Name/DisplayName property) - so it is produced directly by the adapter rather
/// than by <see cref="OptiA11y.Core.Parsing.HtmlFragmentParser"/>. Only emitted for the root
/// content item being audited, not for nested blocks, since a block's name is an editorial
/// label rather than a page title.
/// </summary>
/// <param name="Location">The root content item's location.</param>
/// <param name="DisplayName">The content item's Name/DisplayName, or null if the adapter could not determine one.</param>
public sealed record PageMetadataFragment(
    SourceLocation Location,
    string? DisplayName) : ContentFragment(Location);

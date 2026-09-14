namespace OptiA11y.Core.Model;

/// <summary>
/// Identifies the exact origin of a fragment inside content, so a finding can be
/// resolved back to the property an editor needs to fix. This is the deep-linking
/// payload: it must carry enough information to reconstruct an editor URL even
/// when the fragment originates from a block nested inside a content area.
/// </summary>
/// <param name="ContentReference">
/// The identifier of the top-level content item (PaaS content GUID/ID, or SaaS content key).
/// </param>
/// <param name="PropertyName">The name of the property on <paramref name="ContentReference"/> that owns this fragment.</param>
/// <param name="BlockPath">
/// The path of nested block references leading from the content item's property to the fragment,
/// empty when the fragment sits directly on the content item. Ordered outermost to innermost,
/// e.g. ["MainContentArea/Block:abc123", "NestedArea/Block:def456"].
/// </param>
/// <param name="Ordinal">The zero-based position of the fragment within its immediate owning property or block, used to disambiguate repeated fragment types (e.g. the third image in a rich text property).</param>
public sealed record SourceLocation(
    string ContentReference,
    string PropertyName,
    IReadOnlyList<string> BlockPath,
    int Ordinal)
{
    public static SourceLocation OnProperty(string contentReference, string propertyName, int ordinal = 0) =>
        new(contentReference, propertyName, Array.Empty<string>(), ordinal);

    /// <summary>
    /// Returns a new <see cref="SourceLocation"/> with an additional segment appended to
    /// <see cref="BlockPath"/>. Adapters use this while recursing into nested blocks/content
    /// areas so each fragment records the full path back to its container.
    /// </summary>
    public SourceLocation WithBlockSegment(string segment, int ordinal)
    {
        var newPath = new List<string>(BlockPath) { segment };
        return this with { BlockPath = newPath, Ordinal = ordinal };
    }

    /// <summary>
    /// A human-readable, stable identifier suitable for grouping and diagnostics.
    /// Not itself an editor URL — hosts reconstruct the actual editor link from the
    /// structured fields above via <see cref="IEditorLinkResolver"/>, since the URL shape
    /// differs between PaaS (edit view route) and SaaS (CMS-specific deep link).
    /// </summary>
    public string ToPathString()
    {
        var path = BlockPath.Count == 0
            ? PropertyName
            : $"{PropertyName}/{string.Join('/', BlockPath)}";

        return $"{ContentReference}::{path}[{Ordinal}]";
    }
}

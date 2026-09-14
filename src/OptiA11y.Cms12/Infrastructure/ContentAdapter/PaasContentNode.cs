namespace OptiA11y.Cms12.Infrastructure.ContentAdapter;

/// <summary>
/// Minimal representation of a PaaS content property used by <see cref="PaasContentAdapter"/>.
///
/// NOTE: this deliberately does not reference EPiServer.CMS.Core types. The NuGet feed for
/// EPiServer.CMS.Core is not resolvable from this environment (confirmed 404 against both
/// nuget.org and nuget.optimizely.com), so this adapter is built against a narrow shape that
/// mirrors the real IContent/PropertyData surface. When the real package is available, replace
/// <see cref="PaasContentNode"/>/<see cref="PaasProperty"/> with adapters over the actual
/// IContent/IContentLoader APIs — the fragment-building logic below does not need to change,
/// only the code that populates these shapes.
/// </summary>
/// <param name="Name">The property name as it appears on the content type.</param>
/// <param name="Kind">What kind of property this is, driving how it is parsed into fragments.</param>
/// <param name="Html">
/// Raw HTML for XhtmlString-like properties. Null for non-HTML property kinds.
/// </param>
/// <param name="Blocks">Nested blocks for ContentArea-like properties, empty otherwise.</param>
public sealed record PaasProperty(
    string Name,
    PaasPropertyKind Kind,
    string? Html,
    IReadOnlyList<PaasContentNode> Blocks);

public enum PaasPropertyKind
{
    XhtmlString,
    ContentArea
}

/// <summary>
/// Minimal representation of a PaaS content item or block, standing in for IContent/BlockData
/// until the real EPiServer.CMS.Core package can be restored in this environment.
/// </summary>
/// <param name="ContentReference">The content/block identifier.</param>
/// <param name="BlockName">The display name for this node when it is a nested block, used to build the block path segment. Null for the top-level content item.</param>
/// <param name="Properties">The properties on this content item or block.</param>
/// <param name="DisplayName">The content item's Name, used by <see cref="OptiA11y.Core.Rules.PageTitle.PageTitleRule"/>. Populated for every node (root and blocks alike); only the root's value is used for that rule.</param>
public sealed record PaasContentNode(
    string ContentReference,
    string? BlockName,
    IReadOnlyList<PaasProperty> Properties,
    string? DisplayName = null);

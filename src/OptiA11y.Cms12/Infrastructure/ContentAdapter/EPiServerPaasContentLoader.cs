using EPiServer;
using EPiServer.Core;

namespace OptiA11y.Cms12.Infrastructure.ContentAdapter;

/// <summary>
/// Real implementation of <see cref="IPaasContentLoader"/> backed by EPiServer's
/// <see cref="IContentLoader"/>. Maps <c>XhtmlString</c> and <see cref="ContentArea"/>
/// properties on the loaded <see cref="IContent"/> into the CMS-agnostic
/// <see cref="PaasContentNode"/>/<see cref="PaasProperty"/> shapes consumed by
/// <see cref="PaasContentAdapter"/>.
/// </summary>
public sealed class EPiServerPaasContentLoader : IPaasContentLoader
{
    private readonly IContentLoader _contentLoader;

    public EPiServerPaasContentLoader(IContentLoader contentLoader)
    {
        _contentLoader = contentLoader;
    }

    public Task<PaasContentNode?> LoadAsync(string contentReference, CancellationToken cancellationToken)
    {
        if (!TryParseContentReference(contentReference, out var reference))
        {
            return Task.FromResult<PaasContentNode?>(null);
        }

        if (!_contentLoader.TryGet<IContent>(reference, out var content))
        {
            return Task.FromResult<PaasContentNode?>(null);
        }

        var node = MapContent(content, blockName: null);
        return Task.FromResult<PaasContentNode?>(node);
    }

    /// <summary>
    /// Parses a content reference string as sent by the CMS editor shell or a direct/deep link.
    /// The shell's context-aware iframe widget can supply values in a few shapes depending on
    /// context (e.g. plain ids like <c>"7"</c>, PaaS composite ids like <c>"7_8"</c>, or values
    /// carrying a trailing work-id/version marker such as <c>"7_8__CatchAll"</c> or a URL-encoded
    /// form). This normalizes those shapes before delegating to
    /// <see cref="ContentReference.TryParse(string, out ContentReference)"/> so a slightly
    /// non-canonical value doesn't fail to resolve.
    /// </summary>
    public static bool TryParseContentReference(string? contentReference, out ContentReference reference)
    {
        reference = ContentReference.EmptyReference;

        if (string.IsNullOrWhiteSpace(contentReference))
        {
            return false;
        }

        var candidate = Uri.UnescapeDataString(contentReference).Trim();

        // Some shell contexts append a "__" separated suffix (e.g. a work-id, view mode, or
        // provider marker) after the actual content reference - strip it before parsing.
        var suffixIndex = candidate.IndexOf("__", StringComparison.Ordinal);
        if (suffixIndex > 0)
        {
            candidate = candidate[..suffixIndex];
        }

        if (ContentReference.TryParse(candidate, out reference))
        {
            return true;
        }

        // Fall back to just the numeric id segment (e.g. "7_8" -> "7") in case the composite
        // work-id portion isn't recognised by TryParse in this context.
        var underscoreIndex = candidate.IndexOf('_');
        if (underscoreIndex > 0)
        {
            var idSegment = candidate[..underscoreIndex];
            if (ContentReference.TryParse(idSegment, out reference))
            {
                return true;
            }
        }

        return false;
    }

    private PaasContentNode MapContent(IContent content, string? blockName)
    {
        var properties = new List<PaasProperty>();

        foreach (PropertyData property in content.Property)
        {
            switch (property.Value)
            {
                case ContentArea contentArea:
                    var blocks = new List<PaasContentNode>();
                    foreach (var item in contentArea.Items)
                    {
                        if (_contentLoader.TryGet<IContent>(item.ContentLink, out var blockContent))
                        {
                            blocks.Add(MapContent(blockContent, blockContent.Name));
                        }
                    }

                    properties.Add(new PaasProperty(
                        property.Name,
                        PaasPropertyKind.ContentArea,
                        Html: null,
                        Blocks: blocks));
                    break;

                case XhtmlString xhtml:
                    properties.Add(new PaasProperty(
                        property.Name,
                        PaasPropertyKind.XhtmlString,
                        xhtml.ToInternalString(),
                        Array.Empty<PaasContentNode>()));
                    break;
            }
        }

        return new PaasContentNode(content.ContentLink.ToString(), blockName, properties, content.Name);
    }
}

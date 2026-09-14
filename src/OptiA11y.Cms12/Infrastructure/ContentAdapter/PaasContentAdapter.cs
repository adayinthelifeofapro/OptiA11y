using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;
using OptiA11y.Cms12.Features.RunAudit;

namespace OptiA11y.Cms12.Infrastructure.ContentAdapter;

/// <summary>
/// Builds an <see cref="AuditDocument"/> from a PaaS content item, recursing into nested blocks
/// inside content areas so every fragment carries an accurate <see cref="SourceLocation"/> back
/// to the property/block it came from — this is the deep-linking foundation.
/// </summary>
public sealed class PaasContentAdapter : IContentAuditDocumentAdapter
{
    private readonly IPaasContentLoader _loader;

    public PaasContentAdapter(IPaasContentLoader loader)
    {
        _loader = loader;
    }

    public async Task<AuditDocument?> BuildAsync(string contentReference, CancellationToken cancellationToken)
    {
        var root = await _loader.LoadAsync(contentReference, cancellationToken);
        if (root is null)
        {
            return null;
        }

        var fragments = new List<ContentFragment>
        {
            new PageMetadataFragment(SourceLocation.OnProperty(root.ContentReference, "Name"), root.DisplayName)
        };
        CollectFragments(root, blockPath: Array.Empty<string>(), fragments);

        return new AuditDocument(contentReference, fragments);
    }

    private static void CollectFragments(PaasContentNode node, IReadOnlyList<string> blockPath, List<ContentFragment> fragments)
    {
        foreach (var property in node.Properties)
        {
            var propertyLocation = new SourceLocation(node.ContentReference, property.Name, blockPath, 0);

            switch (property.Kind)
            {
                case PaasPropertyKind.XhtmlString:
                    if (property.Html is not null)
                    {
                        fragments.AddRange(HtmlFragmentParser.Parse(property.Html, propertyLocation));
                    }
                    break;

                case PaasPropertyKind.ContentArea:
                    foreach (var block in property.Blocks)
                    {
                        var segment = $"{property.Name}:{block.BlockName ?? block.ContentReference}";
                        var nestedPath = blockPath.Append(segment).ToList();
                        CollectFragments(block, nestedPath, fragments);
                    }
                    break;
            }
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;
using OptiA11y.Core.Rules;

namespace OptiA11y.Saas.ContentApi;

/// <summary>
/// Builds an <see cref="AuditDocument"/> from the SaaS Content API's JSON response. Produces the
/// same document shape as <c>PaasContentAdapter</c> in OptiA11y.Cms, so the same
/// <see cref="RuleEngine"/> and rule set serve both flavours without modification — this is the
/// resolution to the "both-flavours" problem in the plan.
/// </summary>
public sealed class SaasContentAdapter
{
    private readonly ISaasContentApiClient _client;

    public SaasContentAdapter(ISaasContentApiClient client)
    {
        _client = client;
    }

    public async Task<AuditDocument?> BuildAsync(string contentReference, CancellationToken cancellationToken)
    {
        var root = await _client.GetContentAsync(contentReference, cancellationToken);
        if (root is null)
        {
            return null;
        }

        var fragments = new List<ContentFragment>();
        CollectFragments(root, blockPath: Array.Empty<string>(), fragments);

        return new AuditDocument(contentReference, fragments);
    }

    private static void CollectFragments(SaasContentResponse node, IReadOnlyList<string> blockPath, List<ContentFragment> fragments)
    {
        foreach (var property in node.Properties)
        {
            var propertyLocation = new SourceLocation(node.ContentLink, property.Name, blockPath, 0);

            if (property.Html is not null)
            {
                fragments.AddRange(HtmlFragmentParser.Parse(property.Html, propertyLocation));
            }

            if (property.ContentArea is not null)
            {
                foreach (var block in property.ContentArea)
                {
                    var segment = $"{property.Name}:{block.ContentLink}";
                    var nestedPath = blockPath.Append(segment).ToList();
                    CollectFragments(block, nestedPath, fragments);
                }
            }
        }
    }
}

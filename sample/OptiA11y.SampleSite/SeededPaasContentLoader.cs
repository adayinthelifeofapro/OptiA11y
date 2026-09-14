using OptiA11y.Cms.Infrastructure.ContentAdapter;

namespace OptiA11y.SampleSite;

/// <summary>
/// A tiny hardcoded "content repository" standing in for IContentLoader, seeded with
/// deliberate accessibility problems — including a nested block — so the RunAudit endpoint
/// and its deep-linking output can be exercised end to end without a real CMS.
/// </summary>
public sealed class SeededPaasContentLoader : IPaasContentLoader
{
    private readonly Dictionary<string, PaasContentNode> _content;

    public SeededPaasContentLoader()
    {
        var teaserBlock = new PaasContentNode(
            ContentReference: "block-teaser-1",
            BlockName: "SummerSaleTeaser",
            Properties: new[]
            {
                new PaasProperty(
                    "TeaserBody",
                    PaasPropertyKind.XhtmlString,
                    // Missing alt entirely, plus a non-descriptive link — both flagged, with
                    // a SourceLocation carrying the nested block path back to this teaser.
                    "<img src='/images/sale-banner.jpg' />" +
                    "<a href='/summer-sale/terms.pdf'>click here</a>",
                    Array.Empty<PaasContentNode>())
            });

        var homePage = new PaasContentNode(
            ContentReference: "page-home",
            BlockName: null,
            Properties: new[]
            {
                new PaasProperty(
                    "MainBody",
                    PaasPropertyKind.XhtmlString,
                    // Skipped heading level (H1 -> H3), redundant alt phrasing, filename-as-alt,
                    // and a raw URL as link text — a spread of every rule's heuristics and facts.
                    "<h1>Welcome to Acme Outdoors</h1>" +
                    "<h3>Our latest gear</h3>" +
                    "<img src='/images/kayak.jpg' alt='image of a kayak' />" +
                    "<img src='/images/IMG_04213.jpg' alt='IMG_04213.jpg' />" +
                    "<a href='https://acme-outdoors.example.com/catalog'>https://acme-outdoors.example.com/catalog</a>",
                    Array.Empty<PaasContentNode>()),
                new PaasProperty(
                    "MainContentArea",
                    PaasPropertyKind.ContentArea,
                    Html: null,
                    Blocks: new[] { teaserBlock })
            });

        _content = new Dictionary<string, PaasContentNode>(StringComparer.OrdinalIgnoreCase)
        {
            [homePage.ContentReference] = homePage
        };
    }

    public Task<PaasContentNode?> LoadAsync(string contentReference, CancellationToken cancellationToken) =>
        Task.FromResult(_content.TryGetValue(contentReference, out var node) ? node : null);
}

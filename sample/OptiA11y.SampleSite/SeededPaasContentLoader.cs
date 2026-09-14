using OptiA11y.Cms.Infrastructure.ContentAdapter;

namespace OptiA11y.SampleSite;

/// <summary>
/// A tiny hardcoded "content repository" standing in for IContentLoader, seeded with
/// deliberate accessibility problems — including a nested block — so the RunAudit endpoint
/// and its deep-linking output can be exercised end to end without a real CMS. Covers every
/// non-rendered-style rule at least once; the five rendered-style-only rules
/// (target-size/focus-indicator/motion/reflow/text-spacing) need a real browser and preview URL,
/// which this sample deliberately doesn't wire up (see OptiA11y.Rendering's own README instead).
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
                    // Missing alt entirely, plus a non-descriptive link to a document with no
                    // format hint — both flagged, with a SourceLocation carrying the nested
                    // block path back to this teaser.
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
                    "<h1>Welcome to Acme Outdoors</h1>" +
                    "<h3>Our latest gear</h3>" +
                    // Redundant alt phrasing, filename-as-alt, and a raw URL as link text.
                    "<img src='/images/kayak.jpg' alt='image of a kayak' />" +
                    "<img src='/images/IMG_04213.jpg' alt='IMG_04213.jpg' />" +
                    "<a href='https://acme-outdoors.example.com/catalog'>https://acme-outdoors.example.com/catalog</a>" +
                    // A link with no accessible name at all - no text, no aria-label, no alt.
                    "<a href='/wishlist'></a>" +
                    // A link whose title just repeats its visible text.
                    "<a href='/returns' title='Returns policy'>Returns policy</a>" +
                    // A sensory-characteristics instruction and a foreign-script passage with no lang override.
                    "<p>Click the green button on the right to check out, or say добро пожаловать to returning customers.</p>" +
                    // A short, fully-bold paragraph being used as a heading.
                    "<p><strong>Shipping &amp; returns</strong></p>" +
                    // An inline SVG icon with no accessible name and no aria-hidden.
                    "<svg viewBox='0 0 24 24'><path d='M12 2 L2 22 L22 22 Z' /></svg>" +
                    // A button nested inside a link - an invalid interaction model.
                    "<a href='/deals'>See today's deals <button>Shop now</button></a>" +
                    // An invalid role and a misspelled aria-* attribute.
                    "<div role='banenr' aria-lable='Promo'>Free shipping over $50</div>" +
                    // An aria-labelledby reference that doesn't resolve to anything on this page.
                    "<div aria-labelledby='promo-heading'>20% off kayaks this week</div>" +
                    // A table with merged header cells and no scope to disambiguate them.
                    "<table><tr><th colspan='2'>Size guide</th></tr><tr><td>S</td><td>32-34in</td></tr></table>" +
                    // Autoplaying, unmuted video with no controls and no audio-description track.
                    "<video src='/videos/gear-demo.mp4' autoplay></video>" +
                    // An orphaned radio group (shared name, no wrapping fieldset) and an
                    // email-looking field with no autocomplete attribute.
                    "<input type='radio' name='delivery' value='standard' />Standard" +
                    "<input type='radio' name='delivery' value='express' />Express" +
                    "<input type='text' name='customer-email' placeholder='Email address' />",
                    Array.Empty<PaasContentNode>()),
                new PaasProperty(
                    "SecondaryBody",
                    PaasPropertyKind.XhtmlString,
                    // Deprecated presentational markup, and a positive tabindex overriding
                    // natural focus order.
                    "<center>All prices in USD</center>" +
                    "<div tabindex='3'>Newsletter signup</div>" +
                    // A button with no accessible name at all, and an iframe with no title.
                    "<button></button>" +
                    "<iframe src='https://maps.example.com/store-locator'></iframe>" +
                    // Manually bulleted text that should be a real list.
                    "<p>- Kayaks\n- Paddles\n- Life jackets</p>" +
                    // Low-contrast and fully-justified/tiny inline-styled text.
                    "<p style='color:#777777;background-color:#666666;'>Store hours vary by location</p>" +
                    "<p style='text-align:justify;'>Return items within thirty days of purchase for a full refund, provided the original packaging and receipt are included with the shipment.</p>" +
                    "<span style='font-size:9px;'>Terms and conditions apply</span>" +
                    // A dense, jargon-heavy passage that scores as very difficult to read.
                    "<p>Notwithstanding the aforementioned considerations, the organizational infrastructure " +
                    "necessitates a comprehensive reevaluation of preexisting fulfilment methodologies in " +
                    "order to accommodate the multifaceted, interdisciplinary logistical requirements " +
                    "engendered by unprecedented seasonal demand fluctuations across substantially all " +
                    "distribution channels within the multinational retail conglomerate.</p>" +
                    // A full document (not just a fragment) to exercise the lang-attribute check.
                    "<html lang='EN_US'>Storefront</html>",
                    Array.Empty<PaasContentNode>()),
                new PaasProperty(
                    "MainContentArea",
                    PaasPropertyKind.ContentArea,
                    Html: null,
                    Blocks: new[] { teaserBlock })
            },
            // A placeholder content name, deliberately left un-renamed - flagged by page-title.
            DisplayName: "New Page");

        _content = new Dictionary<string, PaasContentNode>(StringComparer.OrdinalIgnoreCase)
        {
            [homePage.ContentReference] = homePage
        };
    }

    public Task<PaasContentNode?> LoadAsync(string contentReference, CancellationToken cancellationToken) =>
        Task.FromResult(_content.TryGetValue(contentReference, out var node) ? node : null);
}

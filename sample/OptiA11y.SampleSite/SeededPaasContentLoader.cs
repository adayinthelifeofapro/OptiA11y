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
                    "<input type='text' name='customer-email' placeholder='Email address' />" +
                    // A generic, placeholder-like label - label-quality.
                    "<label for='field-1'>Field 1</label><input type='text' id='field-1' name='promo-code' />" +
                    // A select whose first option is a "please choose" placeholder, not a real value.
                    "<select name='size'><option>Please select</option><option>Small</option><option>Large</option></select>" +
                    // A visually-required field (asterisk in the label) with no required/aria-required attribute.
                    "<label for='field-2'>Gift message *</label><input type='text' id='field-2' name='gift-message' />" +
                    // A "confirm email" field with no autocomplete, asking the user to redundantly re-enter data.
                    "<label for='field-3'>Confirm email</label><input type='text' id='field-3' name='confirm-email' />" +
                    // A field flagged invalid with no aria-describedby pointing at an error message.
                    "<label for='field-4'>Coupon code</label><input type='text' id='field-4' name='coupon-code' aria-invalid='true' />" +
                    // A pattern-constrained field with no aria-describedby explaining the expected format.
                    "<label for='field-5'>Postal code</label><input type='text' id='field-5' name='postal-code' pattern='[0-9]{5}' />",
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
                    "<html lang='EN_US'>Storefront</html>" +
                    // A figure whose alt text reads as a full quoted sentence - suggests text baked into the image.
                    "<figure><img src='/images/quote-graphic.png' alt='Adventure is worthwhile in itself, as the saying goes among seasoned travellers.' /></figure>" +
                    // A captioned figure whose image is nonetheless marked decorative (empty alt).
                    "<figure><img src='/images/store-front.jpg' alt='' /><figcaption>Our flagship store in downtown Seattle</figcaption></figure>" +
                    // A figure whose caption exactly duplicates its alt text - redundant announcement.
                    "<figure><img src='/images/team-photo.jpg' alt='The team at the summit' /><figcaption>The team at the summit</figcaption></figure>" +
                    // A chart-like image with only a short label for alt text.
                    "<img src='/images/sales-chart.png' alt='Sales chart' />" +
                    // A manually numbered list with a stray non-<li> child - list-misuse.
                    "<ul><li>Kayaks</li><div>Ad: free shipping today</div><li>Paddles</li></ul>" +
                    // An empty list - list-misuse.
                    "<ol></ol>" +
                    // A definition list with an orphaned term and a description before any term.
                    "<dl><dd>Waterproof storage bag</dd><dt>Dry bag</dt><dt>Paddle leash</dt></dl>" +
                    // A visual-layout table with no header row and no caption.
                    "<table><tr><td>Kayaks</td><td>Paddles</td></tr><tr><td>$399</td><td>$59</td></tr></table>" +
                    // Two elements sharing the same id - duplicate-id.
                    "<div id='promo-banner'>Summer sale</div><div id='promo-banner'>Ends soon</div>" +
                    // A short, non-quotation blockquote with no citation.
                    "<blockquote>Great store!</blockquote>" +
                    // An overly long heading that reads as body copy.
                    "<h2>Our new lineup of kayaks, paddles, and life jackets is now available in every store location nationwide for the summer season</h2>" +
                    // A long run of text wrapped entirely in bold rather than a semantic element.
                    "<b>All sales are final once the order has shipped and cannot be returned, exchanged, or refunded under any circumstances after that point.</b>" +
                    // A long passage wrapped in superscript, suggesting styling rather than true superscript use.
                    "<sup>See store for full terms and conditions applicable to this promotional offer</sup>" +
                    // Text in an absolutely positioned element - meaningful-sequence.
                    "<div style='position:absolute;'>Limited time offer - ends Sunday</div>" +
                    // Same destination, different link text - same-destination-different-text.
                    "<p><a href='/product/kayak'>Read the kayak review</a> <a href='/product/kayak'>Download the spec sheet</a></p>" +
                    // A link that opens in a new tab with no warning in its text - new-window-link.
                    "<a href='https://partner-outfitters.example.com' target='_blank'>Visit our partner outfitters</a>" +
                    // An image link immediately followed by a text link to the same destination - adjacent-duplicate-links.
                    "<a href='/product/paddle'><img src='/images/paddle.jpg' alt='' /></a><a href='/product/paddle'>Carbon Fibre Paddle</a>" +
                    // An in-page anchor link whose target id doesn't exist anywhere in this markup - skip-link-target.
                    "<a href='#store-hours'>Jump to store hours</a>" +
                    // A meta refresh tag - meta-refresh.
                    "<meta http-equiv='refresh' content='10;url=/new-storefront' />" +
                    // role='list' with no listitem descendants - aria-required-children.
                    "<div role='list'><span>Kayaks</span><span>Paddles</span></div>" +
                    // role='checkbox' missing aria-checked - aria-required-attributes.
                    "<div role='checkbox' tabindex='0'>Subscribe to newsletter</div>" +
                    // aria-checked on a role='button' element, which doesn't support it - aria-allowed-attribute.
                    "<div role='button' aria-checked='true' tabindex='0'>Add to cart</div>" +
                    // role duplicating the element's native implicit role - redundant-role.
                    "<nav role='navigation'>Site navigation</nav>" +
                    // aria-hidden container that still contains a focusable button - aria-hidden-focusable.
                    "<div aria-hidden='true'><button>Hidden but focusable</button></div>" +
                    // role='presentation' on a focusable link - presentation-role-conflict.
                    "<a href='/promo' role='presentation'>Shop the summer promo</a>" +
                    // a 'main' landmark embedded directly in content markup - landmark-structure.
                    "<main>Featured products this week</main>" +
                    // aria-live with an invalid politeness value - live-region-misuse.
                    "<div aria-live='eventually'>Cart updated</div>" +
                    // A repeated acronym with no expansion anywhere - abbreviation-expansion.
                    "<p>Read our FAQ before contacting support. The FAQ covers shipping and returns.</p>" +
                    // Corporate jargon/idiom - unusual-words.
                    "<p>Our team is working to move the needle on customer satisfaction this quarter.</p>" +
                    // A heteronym whose pronunciation depends on meaning - pronunciation-ambiguity.
                    "<p>Please wind the strap tightly before storing your kayak.</p>" +
                    // Mathematical-alphanumeric 'fancy font' characters faking bold styling - unicode-styled-text.
                    "<p>\U0001D5D5\U0001D5EE\U0001D5F5\U0001D5F2 sale this weekend only</p>" +
                    // A repeated emoji run used for decoration - emoji-overuse.
                    "<p>Summer sale is here \U0001F389\U0001F389\U0001F389</p>" +
                    // A punctuation-heavy divider line - ascii-art.
                    "<p>----------------------------------------</p>" +
                    // Repeated non-breaking spaces faking column alignment - whitespace-formatting.
                    "<p>Kayaks\u00A0\u00A0\u00A0\u00A0$399</p>" +
                    // Consecutive <br> elements faking paragraph spacing - line-break-misuse.
                    "<p>Store hours today.<br /><br /><br />See you soon!</p>" +
                    // Link text in a different script with no lang override - link-text-language.
                    "<a href='/international'>Добро пожаловать</a>" +
                    // Deprecated blinking element - blinking-content.
                    "<blink>Sale ends tonight!</blink>" +
                    // Session-expiry copy with no visible way to extend it - timed-content.
                    "<p>Your session will time out in 5 minutes. Please finish checking out.</p>" +
                    // A 'transcript' link pointing at the video file itself - media-transcript-quality.
                    "<video src='/videos/product-tour.mp4' controls></video>" +
                    "<a href='/videos/product-tour.mp4'>transcript</a>" +
                    // Video with no sign-language track reference - sign-language.
                    "<video src='/videos/warranty-info.mp4' controls><track kind='captions' src='/videos/warranty-info.vtt' /></video>" +
                    // Video with a standard description track but no extended-description track - extended-audio-description.
                    "<video src='/videos/assembly-guide.mp4' controls><track kind='descriptions' src='/videos/assembly-guide-ad.vtt' /></video>" +
                    // Video with neither an audio-description track nor a transcript/captions - media-alternative.
                    "<video src='/videos/factory-tour.mp4' controls></video>" +
                    // Media source naming referencing a strobe effect - flashing-content.
                    "<video src='/videos/strobe-light-show.mp4' controls></video>" +
                    // Instruction referencing color alone - use-of-color.
                    "<p>Required fields are shown in red.</p>" +
                    // A link styled with color only, underline removed - link-distinguishability.
                    "<a href='/details' style='color: #cc0000; text-decoration: none;'>View details</a>" +
                    // Low-contrast inline-styled border/icon fill - non-text-contrast.
                    "<div style='border-color: #cccccc;'></div>" +
                    // Text below the AAA-enhanced 7:1 threshold - contrast-enhanced.
                    "<p style='color: #767676; background-color: #ffffff;'>Shipping details</p>" +
                    // Very wide inline-styled text block - line-length.
                    "<p style='width: 1200px;'>Our return policy allows exchanges within thirty days of purchase, provided the item is unused and in its original packaging with all tags attached.</p>" +
                    // Tight line-height on body copy - line-spacing.
                    "<p style='line-height: 1.1;'>Kayak paddles are sold separately and ship within two business days of order confirmation.</p>" +
                    // Text overlaid directly on a background image - background-image-text.
                    "<p style='background-image: url(/images/hero-banner.jpg);'>Summer clearance event</p>",
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

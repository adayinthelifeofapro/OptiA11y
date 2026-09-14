using Microsoft.Playwright;

namespace OptiA11y.Rendering;

/// <summary>
/// Captures rendered text styles using headless Chromium via Playwright. This is the only class
/// in the solution that depends on a real browser engine; everything downstream (fragment
/// building, rules) stays pure and testable without one.
///
/// Requires Playwright's browser binaries to be installed once per machine/build agent via
/// <c>pwsh bin/Debug/net10.0/playwright.ps1 install chromium</c> (or the equivalent
/// <c>playwright install chromium</c> CLI call). If the browser cannot be launched, capture
/// fails soft and returns an empty list so callers can treat this as a best-effort enrichment.
/// </summary>
public sealed class PlaywrightRenderedStyleProvider : IRenderedStyleProvider, IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task<IReadOnlyList<RenderedTextStyle>> CaptureAsync(Uri pageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var browser = await GetOrCreateBrowserAsync();
            var page = await browser.NewPageAsync();
            try
            {
                await page.GotoAsync(pageUrl.ToString(), new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                var results = await page.EvaluateAsync<RenderedTextStyleDto[]>(ExtractionScript);

                return results
                    .Where(r => !string.IsNullOrWhiteSpace(r.Text))
                    .Select(r => new RenderedTextStyle(
                        r.Text.Trim(),
                        r.Color,
                        r.BackgroundColor,
                        r.FontSizePx,
                        r.FontWeight,
                        r.TextAlign))
                    .ToList();
            }
            finally
            {
                await page.CloseAsync();
            }
        }
        catch
        {
            // Rendering is a best-effort enrichment; any failure (browser unavailable,
            // navigation timeout, unreachable URL) should not fail the whole audit.
            return Array.Empty<RenderedTextStyle>();
        }
    }

    private async Task<IBrowser> GetOrCreateBrowserAsync()
    {
        if (_browser is not null)
        {
            return _browser;
        }

        await _initLock.WaitAsync();
        try
        {
            _browser ??= await ((_playwright ??= await Playwright.CreateAsync())
                .Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }));
            return _browser;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    // Walks visible text nodes, resolving the computed foreground color, the nearest opaque
    // ancestor background, font size, font weight, and text-align for each. Kept as a single
    // inline script (rather than multiple round-trips) for performance on content-heavy pages.
    private const string ExtractionScript = """
        () => {
            function isVisible(el) {
                const style = window.getComputedStyle(el);
                if (style.display === 'none' || style.visibility === 'hidden' || parseFloat(style.opacity) === 0) {
                    return false;
                }
                const rect = el.getBoundingClientRect();
                return rect.width > 0 && rect.height > 0;
            }

            function effectiveBackground(el) {
                let node = el;
                while (node) {
                    const style = window.getComputedStyle(node);
                    const bg = style.backgroundColor;
                    if (bg && bg !== 'rgba(0, 0, 0, 0)' && bg !== 'transparent') {
                        return bg;
                    }
                    node = node.parentElement;
                }
                return 'rgb(255, 255, 255)';
            }

            const results = [];
            const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT, null);
            let node;
            while ((node = walker.nextNode())) {
                const text = node.textContent ? node.textContent.trim() : '';
                if (!text) {
                    continue;
                }
                const el = node.parentElement;
                if (!el || !isVisible(el)) {
                    continue;
                }
                const style = window.getComputedStyle(el);
                results.push({
                    text: text,
                    color: style.color,
                    backgroundColor: effectiveBackground(el),
                    fontSizePx: parseFloat(style.fontSize) || 0,
                    fontWeight: style.fontWeight,
                    textAlign: style.textAlign
                });
            }
            return results;
        }
        """;

    private sealed class RenderedTextStyleDto
    {
        public string Text { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public double FontSizePx { get; set; }
        public string FontWeight { get; set; } = string.Empty;
        public string TextAlign { get; set; } = string.Empty;
    }
}

using Microsoft.Playwright;

namespace OptiA11y.Rendering;

public sealed class PlaywrightRenderedStyleProvider : IRenderedStyleProvider, IAsyncDisposable
{
    private const int NarrowViewportWidthPx = 320;

    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task<RenderedPageDiagnostics> CaptureAsync(Uri pageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            var browser = await GetOrCreateBrowserAsync();
            var page = await browser.NewPageAsync();
            try
            {
                await page.GotoAsync(pageUrl.ToString(), new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                var originalViewport = page.ViewportSize;

                // 1. Default-viewport measurements: text styles, interactive-element diagnostics,
                // and elements with automatic infinite animation.
                var mainPassResult = await page.EvaluateAsync<MainPassResultDto>(MainPassScript);

                var textStyles = mainPassResult.TextStyles
                    .Where(r => !string.IsNullOrWhiteSpace(r.Text))
                    .Select(r => new RenderedTextStyle(
                        r.Text.Trim(),
                        r.Color,
                        r.BackgroundColor,
                        r.FontSizePx,
                        r.FontWeight,
                        r.TextAlign,
                        r.HasBackgroundImage))
                    .ToList();

                var elements = mainPassResult.Elements
                    .Select(e => new RenderedElementDiagnostics(e.Description, e.WidthPx, e.HeightPx, e.HasVisibleFocusIndicator))
                    .ToList();

                var animatedElementDescriptions = mainPassResult.AnimatedElementDescriptions.ToList();

                // 2. Reflow: resize to a 320px-equivalent viewport and check for horizontal
                // overflow, then restore the original viewport.
                await page.SetViewportSizeAsync(NarrowViewportWidthPx, originalViewport?.Height ?? 720);
                var overflowsAtNarrowViewport = await page.EvaluateAsync<bool>(ReflowCheckScript);
                await page.SetViewportSizeAsync(originalViewport?.Width ?? 1280, originalViewport?.Height ?? 720);

                // 3. Text spacing: inject the WCAG 1.4.12 reference overrides and check which
                // text samples clip as a result.
                await page.AddStyleTagAsync(new PageAddStyleTagOptions { Content = TextSpacingOverrideCss });
                var textSpacingClippedSamples = (await page.EvaluateAsync<string[]>(TextSpacingCheckScript)).ToList();

                return new RenderedPageDiagnostics(textStyles, elements, animatedElementDescriptions, overflowsAtNarrowViewport, textSpacingClippedSamples);
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
            return RenderedPageDiagnostics.Empty;
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

    // Walks visible text nodes (for contrast/readability) and a fixed set of interactive
    // element candidates (for target-size/focus-indicator), plus a full-page animation scan
    // (for motion), all in one round trip for performance on content-heavy pages.
    private const string MainPassScript = """
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
                let hasImage = false;
                while (node) {
                    const style = window.getComputedStyle(node);
                    if (style.backgroundImage && style.backgroundImage !== 'none') {
                        hasImage = true;
                    }
                    const bg = style.backgroundColor;
                    if (bg && bg !== 'rgba(0, 0, 0, 0)' && bg !== 'transparent') {
                        return { color: bg, hasImage: hasImage };
                    }
                    node = node.parentElement;
                }
                return { color: 'rgb(255, 255, 255)', hasImage: hasImage };
            }

            const textStyles = [];
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
                const background = effectiveBackground(el);
                textStyles.push({
                    text: text,
                    color: style.color,
                    backgroundColor: background.color,
                    hasBackgroundImage: background.hasImage,
                    fontSizePx: parseFloat(style.fontSize) || 0,
                    fontWeight: style.fontWeight,
                    textAlign: style.textAlign
                });
            }

            function describe(el) {
                const name = (el.textContent || el.getAttribute('aria-label') || el.getAttribute('alt') || '').trim().slice(0, 40);
                return '<' + el.tagName.toLowerCase() + '> "' + name + '"';
            }

            const focusProperties = ['outlineStyle', 'outlineWidth', 'outlineColor', 'boxShadow', 'backgroundColor', 'borderColor'];
            const previouslyFocused = document.activeElement;
            const elements = [];
            const candidates = document.querySelectorAll('a[href], button, input:not([type="hidden"]), select, textarea, [tabindex]');
            for (const el of candidates) {
                if (!isVisible(el)) {
                    continue;
                }

                const computedDisplay = window.getComputedStyle(el).display;
                const isInlineExempt = computedDisplay.indexOf('inline') === 0 && computedDisplay !== 'inline-block';
                const isNativeCheckboxOrRadioExempt = el.tagName === 'INPUT' && (el.type === 'checkbox' || el.type === 'radio');
                const isTargetSizeExempt = isInlineExempt || isNativeCheckboxOrRadioExempt;

                const rect = el.getBoundingClientRect();
                const widthPx = isTargetSizeExempt ? 9999 : rect.width;
                const heightPx = isTargetSizeExempt ? 9999 : rect.height;

                const restingStyle = window.getComputedStyle(el);
                const restingSnapshot = focusProperties.map(p => restingStyle[p]);
                el.focus({ preventScroll: true });
                const focusedStyle = window.getComputedStyle(el);
                const focusedSnapshot = focusProperties.map(p => focusedStyle[p]);
                el.blur();

                const hasVisibleFocusIndicator = restingSnapshot.some((value, i) => value !== focusedSnapshot[i]);

                elements.push({
                    description: describe(el),
                    widthPx: widthPx,
                    heightPx: heightPx,
                    hasVisibleFocusIndicator: hasVisibleFocusIndicator
                });
            }
            if (previouslyFocused && previouslyFocused.focus) {
                previouslyFocused.focus({ preventScroll: true });
            }

            const animatedElementDescriptions = [];
            for (const el of document.querySelectorAll('*')) {
                const style = window.getComputedStyle(el);
                if (style.animationName && style.animationName !== 'none'
                    && style.animationIterationCount === 'infinite'
                    && style.animationPlayState !== 'paused'
                    && isVisible(el)) {
                    animatedElementDescriptions.push(describe(el));
                }
            }

            return {
                textStyles: textStyles,
                elements: elements,
                animatedElementDescriptions: animatedElementDescriptions
            };
        }
        """;

    private const string ReflowCheckScript = """
        () => document.documentElement.scrollWidth > window.innerWidth
        """;

    // The WCAG 1.4.12 reference stylesheet overrides: line height to 1.5x font size, spacing
    // between paragraphs to 2x font size, letter spacing to 0.12x font size, and word spacing to
    // 0.16x font size.
    private const string TextSpacingOverrideCss = """
        * {
            line-height: 1.5 !important;
            letter-spacing: 0.12em !important;
            word-spacing: 0.16em !important;
        }
        p {
            margin-top: 2em !important;
            margin-bottom: 2em !important;
        }
        """;

    // Re-walks visible text nodes after the overrides are applied and reports any whose parent
    // element clips its content (overflow hidden with content taller/wider than the box).
    private const string TextSpacingCheckScript = """
        () => {
            const clipped = [];
            const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT, null);
            let node;
            while ((node = walker.nextNode())) {
                const text = node.textContent ? node.textContent.trim() : '';
                if (!text) {
                    continue;
                }
                const el = node.parentElement;
                if (!el) {
                    continue;
                }
                const style = window.getComputedStyle(el);
                const rect = el.getBoundingClientRect();
                if (rect.width === 0 || rect.height === 0) {
                    continue;
                }
                const overflowsVertically = el.scrollHeight > el.clientHeight + 1 && (style.overflowY === 'hidden' || style.overflow === 'hidden');
                const overflowsHorizontally = el.scrollWidth > el.clientWidth + 1 && (style.overflowX === 'hidden' || style.overflow === 'hidden');
                if (overflowsVertically || overflowsHorizontally) {
                    clipped.push(text);
                }
            }
            return clipped;
        }
        """;

    private sealed class MainPassResultDto
    {
        public RenderedTextStyleDto[] TextStyles { get; set; } = Array.Empty<RenderedTextStyleDto>();
        public RenderedElementDto[] Elements { get; set; } = Array.Empty<RenderedElementDto>();
        public string[] AnimatedElementDescriptions { get; set; } = Array.Empty<string>();
    }

    private sealed class RenderedTextStyleDto
    {
        public string Text { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public bool HasBackgroundImage { get; set; }
        public double FontSizePx { get; set; }
        public string FontWeight { get; set; } = string.Empty;
        public string TextAlign { get; set; } = string.Empty;
    }

    private sealed class RenderedElementDto
    {
        public string Description { get; set; } = string.Empty;
        public double WidthPx { get; set; }
        public double HeightPx { get; set; }
        public bool HasVisibleFocusIndicator { get; set; }
    }
}

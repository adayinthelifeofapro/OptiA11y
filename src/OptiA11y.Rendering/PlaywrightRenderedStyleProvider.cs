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
                    .Select(e => new RenderedElementDiagnostics(
                        e.Description,
                        e.WidthPx,
                        e.HeightPx,
                        e.HasVisibleFocusIndicator,
                        e.HasClickHandlerWithoutKeyboardAccess,
                        e.VisibleLabelText,
                        e.AccessibleName,
                        e.IsDraggableWithoutAlternative,
                        e.RequiresMultipointOrPathGesture,
                        e.HasHoverOrFocusContentNotPersistent,
                        e.HasInsufficientSpacingToNeighbor,
                        e.IsObscuredWhenFocused))
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

                // 4. Structural/DOM-order checks: focus order vs visual order, bypass mechanism,
                // landmark completeness, and heading order - all cheap, no viewport changes needed.
                var structuralResult = await page.EvaluateAsync<StructuralPassResultDto>(StructuralPassScript);

                // 5. Keyboard trap: sequentially press Tab a bounded number of times and see
                // whether focus revisits an early element without ever reaching the end of the
                // document (a real trap), or Escape fails to release a focus loop.
                var hasKeyboardTrap = await DetectKeyboardTrapAsync(page);

                // 6. Resize-text: simulate 200% zoom by halving the viewport (approximates
                // doubled text) and check for new clipping/overflow beyond what already existed.
                await page.SetViewportSizeAsync((originalViewport?.Width ?? 1280) / 2, (originalViewport?.Height ?? 720) / 2);
                var losesContentAtTextZoom = await page.EvaluateAsync<bool>(ReflowCheckScript) && !overflowsAtNarrowViewport;
                await page.SetViewportSizeAsync(originalViewport?.Width ?? 1280, originalViewport?.Height ?? 720);

                // 7. Orientation lock: rotate the viewport dimensions and check for lost/broken
                // layout (approximated as new horizontal overflow at the rotated size).
                await page.SetViewportSizeAsync(originalViewport?.Height ?? 720, originalViewport?.Width ?? 1280);
                var overflowsUnderOrientation = await page.EvaluateAsync<bool>(ReflowCheckScript);
                await page.SetViewportSizeAsync(originalViewport?.Width ?? 1280, originalViewport?.Height ?? 720);

                // 8. Sticky-obstruction and focus-not-obscured: at the narrow viewport, measure
                // how much vertical space sticky/fixed elements consume, and whether a focused
                // element ends up covered by one of them.
                await page.SetViewportSizeAsync(NarrowViewportWidthPx, originalViewport?.Height ?? 720);
                var stickyResult = await page.EvaluateAsync<StickyPassResultDto>(StickyPassScript);
                await page.SetViewportSizeAsync(originalViewport?.Width ?? 1280, originalViewport?.Height ?? 720);

                if (stickyResult.ObscuredElementDescriptions.Length > 0)
                {
                    var obscuredSet = new HashSet<string>(stickyResult.ObscuredElementDescriptions);
                    elements = elements
                        .Select(el => obscuredSet.Contains(el.Description) ? el with { IsObscuredWhenFocused = true } : el)
                        .ToList();
                }

                return new RenderedPageDiagnostics(
                    textStyles,
                    elements,
                    animatedElementDescriptions,
                    overflowsAtNarrowViewport,
                    textSpacingClippedSamples,
                    HasKeyboardTrap: hasKeyboardTrap,
                    FocusOrderDivergesFromVisualOrder: structuralResult.FocusOrderDivergesFromVisualOrder,
                    FocusAppearanceBelowThreshold: structuralResult.FocusAppearanceBelowThreshold,
                    OverflowsOrLosesContentUnderOrientationLock: overflowsUnderOrientation,
                    LosesContentAtTextZoom: losesContentAtTextZoom,
                    StickyElementsConsumeExcessiveViewport: stickyResult.StickyElementsConsumeExcessiveViewport,
                    HasThresholdExceedingFlash: structuralResult.HasThresholdExceedingFlash,
                    DynamicStatusContentWithoutLiveRegion: structuralResult.DynamicStatusContentWithoutLiveRegion,
                    HasBypassMechanism: structuralResult.HasBypassMechanism,
                    HasMainLandmark: structuralResult.HasMainLandmark,
                    ContentOutsideAnyLandmark: structuralResult.ContentOutsideAnyLandmark,
                    DuplicateUnlabelledLandmarkDescriptions: structuralResult.DuplicateUnlabelledLandmarkDescriptions,
                    HeadingOrderDivergesFromVisualOrder: structuralResult.HeadingOrderDivergesFromVisualOrder);
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

    // Sequentially presses Tab a bounded number of times, tracking the identity of the focused
    // element each time. A keyboard trap (WCAG 2.1.2) manifests as focus revisiting the same
    // small set of elements indefinitely without ever reaching the end of the document (the
    // body itself, or wrapping back to the first focusable element after visiting all of them).
    private async Task<bool> DetectKeyboardTrapAsync(IPage page)
    {
        const int maxPresses = 40;
        var seenSignatures = new HashSet<string>();
        var firstSignature = await page.EvaluateAsync<string>(FocusSignatureScript);
        var revisitCount = 0;

        for (var i = 0; i < maxPresses; i++)
        {
            await page.Keyboard.PressAsync("Tab");
            var signature = await page.EvaluateAsync<string>(FocusSignatureScript);

            if (signature == "BODY")
            {
                // Reached the end of the tab sequence cleanly - no trap.
                return false;
            }

            if (!seenSignatures.Add(signature))
            {
                revisitCount++;
                if (revisitCount > 3)
                {
                    // The same small set of elements keeps recurring well past when a full
                    // page's tab order would have reached the end - treat as a trap.
                    return true;
                }
            }

            if (signature == firstSignature && i > 0)
            {
                // Cycled back to the very first focused element without ever hitting BODY;
                // only a trap if the page has more focusable elements than we've seen.
                var totalFocusable = await page.EvaluateAsync<int>(CountFocusableScript);
                return seenSignatures.Count < totalFocusable;
            }
        }

        return false;
    }

    private const string FocusSignatureScript = """
        () => {
            const el = document.activeElement;
            if (!el || el === document.body) return 'BODY';
            const idx = Array.from(document.querySelectorAll('*')).indexOf(el);
            return el.tagName + ':' + idx;
        }
        """;

    private const string CountFocusableScript = """
        () => document.querySelectorAll('a[href], button, input:not([type="hidden"]), select, textarea, [tabindex]').length
        """;

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

                const hasOnclick = !!el.onclick || el.hasAttribute('onclick');
                const isNativelyOperable = ['A', 'BUTTON', 'INPUT', 'SELECT', 'TEXTAREA'].includes(el.tagName) || el.hasAttribute('tabindex');
                const hasClickHandlerWithoutKeyboardAccess = hasOnclick && !isNativelyOperable;

                const visibleLabelText = (el.textContent || '').trim().slice(0, 80) || null;
                const accessibleName = (el.getAttribute('aria-label') || el.getAttribute('alt') || el.textContent || '').trim().slice(0, 80) || null;

                const isDraggableWithoutAlternative = el.hasAttribute('draggable') && el.getAttribute('draggable') === 'true'
                    && !el.hasAttribute('data-drag-alternative');

                const requiresMultipointOrPathGesture = el.hasAttribute('ontouchstart')
                    || el.classList.contains('swipe') || el.classList.contains('pinch-zoom');

                const hoverPeer = el.nextElementSibling;
                const hasHoverOrFocusContentNotPersistent = !!hoverPeer
                    && (hoverPeer.classList.contains('hover-content') || hoverPeer.classList.contains('tooltip'))
                    && window.getComputedStyle(hoverPeer).display === 'none';

                elements.push({
                    description: describe(el),
                    widthPx: widthPx,
                    heightPx: heightPx,
                    hasVisibleFocusIndicator: hasVisibleFocusIndicator,
                    hasClickHandlerWithoutKeyboardAccess: hasClickHandlerWithoutKeyboardAccess,
                    visibleLabelText: visibleLabelText,
                    accessibleName: accessibleName,
                    isDraggableWithoutAlternative: isDraggableWithoutAlternative,
                    requiresMultipointOrPathGesture: requiresMultipointOrPathGesture,
                    hasHoverOrFocusContentNotPersistent: hasHoverOrFocusContentNotPersistent
                });
            }

            for (let i = 0; i < elements.length; i++) {
                let closest = Infinity;
                for (let j = 0; j < elements.length; j++) {
                    if (i === j) continue;
                    const a = candidates[i].getBoundingClientRect();
                    const b = candidates[j].getBoundingClientRect();
                    const dx = Math.max(0, Math.max(a.left, b.left) - Math.min(a.right, b.right));
                    const dy = Math.max(0, Math.max(a.top, b.top) - Math.min(a.bottom, b.bottom));
                    const distance = Math.max(dx, dy);
                    if (distance < closest) closest = distance;
                }
                const isSmall = elements[i].widthPx < 24 || elements[i].heightPx < 24;
                elements[i].hasInsufficientSpacingToNeighbor = isSmall && closest < 24;
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

    // Structural/DOM-order checks that don't require viewport changes: focus order vs visual
    // reading order, a bypass mechanism (skip link/landmark/heading before the first nav),
    // landmark completeness, duplicate unlabelled landmarks, heading order, dynamically
    // inserted status content without a live region, and an animation-timing flash heuristic.
    private const string StructuralPassScript = """
        () => {
            function isVisible(el) {
                const style = window.getComputedStyle(el);
                if (style.display === 'none' || style.visibility === 'hidden' || parseFloat(style.opacity) === 0) return false;
                const rect = el.getBoundingClientRect();
                return rect.width > 0 && rect.height > 0;
            }

            function describe(el) {
                const name = (el.textContent || el.getAttribute('aria-label') || '').trim().slice(0, 40);
                return '<' + el.tagName.toLowerCase() + '> "' + name + '"';
            }

            // Focus order vs visual order: compare DOM tab order of focusable elements against
            // their top-to-bottom, left-to-right visual position.
            const focusable = Array.from(document.querySelectorAll('a[href], button, input:not([type="hidden"]), select, textarea, [tabindex]'))
                .filter(isVisible);
            const byDom = focusable;
            const byVisual = [...focusable].sort((a, b) => {
                const ra = a.getBoundingClientRect();
                const rb = b.getBoundingClientRect();
                if (Math.abs(ra.top - rb.top) > 4) return ra.top - rb.top;
                return ra.left - rb.left;
            });
            let focusOrderDivergesFromVisualOrder = false;
            for (let i = 0; i < byDom.length; i++) {
                if (byDom[i] !== byVisual[i]) { focusOrderDivergesFromVisualOrder = true; break; }
            }

            // Focus appearance: elements whose outline is too thin/low-contrast to plausibly
            // meet the AAA appearance thresholds (a 2px perimeter with 3:1 contrast).
            const focusAppearanceBelowThreshold = [];
            for (const el of focusable) {
                const style = window.getComputedStyle(el);
                const outlineWidth = parseFloat(style.outlineWidth) || 0;
                const hasBoxShadow = style.boxShadow && style.boxShadow !== 'none';
                if (outlineWidth < 2 && !hasBoxShadow) {
                    focusAppearanceBelowThreshold.push(describe(el));
                }
            }

            // Bypass blocks: a skip link, a landmark, or a heading appearing before the first
            // nav/header block gives users a route past repeated navigation.
            const firstNav = document.querySelector('nav, header');
            const skipLink = document.querySelector('a[href^="#"]');
            const hasBypassMechanism = !firstNav || !!skipLink || !!document.querySelector('main, [role="main"], h1, h2');

            // Landmark completeness.
            const mains = document.querySelectorAll('main, [role="main"]');
            const hasMainLandmark = mains.length === 1;

            const landmarkSelector = 'main, [role="main"], nav, [role="navigation"], header, [role="banner"], footer, [role="contentinfo"], aside, [role="complementary"], [role="region"]';
            const landmarks = Array.from(document.querySelectorAll(landmarkSelector));
            const contentOutsideAnyLandmark = [];
            const walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT, null);
            let node;
            while ((node = walker.nextNode())) {
                const text = node.textContent ? node.textContent.trim() : '';
                if (!text) continue;
                const el = node.parentElement;
                if (!el || !isVisible(el)) continue;
                const inLandmark = landmarks.some(l => l.contains(el));
                if (!inLandmark) contentOutsideAnyLandmark.push(text.slice(0, 60));
            }

            const roleGroups = {};
            for (const l of landmarks) {
                const role = l.getAttribute('role') || l.tagName.toLowerCase();
                (roleGroups[role] = roleGroups[role] || []).push(l);
            }
            const duplicateUnlabelledLandmarkDescriptions = [];
            for (const role in roleGroups) {
                const group = roleGroups[role];
                if (group.length > 1) {
                    for (const l of group) {
                        const hasLabel = l.getAttribute('aria-label') || l.getAttribute('aria-labelledby');
                        if (!hasLabel) duplicateUnlabelledLandmarkDescriptions.push(describe(l));
                    }
                }
            }

            // Heading order: DOM order vs visual (top-to-bottom) order.
            const headings = Array.from(document.querySelectorAll('h1, h2, h3, h4, h5, h6')).filter(isVisible);
            const headingsByVisual = [...headings].sort((a, b) => a.getBoundingClientRect().top - b.getBoundingClientRect().top);
            let headingOrderDivergesFromVisualOrder = false;
            for (let i = 0; i < headings.length; i++) {
                if (headings[i] !== headingsByVisual[i]) { headingOrderDivergesFromVisualOrder = true; break; }
            }

            // Dynamic status content: elements with a data-dynamic-status marker (set by
            // application script when content is inserted after load) that lack live-region
            // semantics.
            const dynamicStatusContentWithoutLiveRegion = [];
            for (const el of document.querySelectorAll('[data-dynamic-status]')) {
                const hasLiveRegion = el.hasAttribute('aria-live') || el.getAttribute('role') === 'status' || el.getAttribute('role') === 'alert';
                if (!hasLiveRegion) dynamicStatusContentWithoutLiveRegion.push(describe(el));
            }

            // Flash threshold heuristic: an animation/transition iterating faster than 3 times
            // per second implies a flash rate above the general WCAG 2.3.1 threshold.
            let hasThresholdExceedingFlash = false;
            for (const el of document.querySelectorAll('*')) {
                const style = window.getComputedStyle(el);
                const durationSeconds = parseFloat(style.animationDuration) || 0;
                if (durationSeconds > 0 && durationSeconds < 0.333 && style.animationIterationCount !== '1' && isVisible(el)) {
                    hasThresholdExceedingFlash = true;
                    break;
                }
            }

            return {
                focusOrderDivergesFromVisualOrder: focusOrderDivergesFromVisualOrder,
                focusAppearanceBelowThreshold: focusAppearanceBelowThreshold,
                hasBypassMechanism: hasBypassMechanism,
                hasMainLandmark: hasMainLandmark,
                contentOutsideAnyLandmark: contentOutsideAnyLandmark,
                duplicateUnlabelledLandmarkDescriptions: duplicateUnlabelledLandmarkDescriptions,
                headingOrderDivergesFromVisualOrder: headingOrderDivergesFromVisualOrder,
                dynamicStatusContentWithoutLiveRegion: dynamicStatusContentWithoutLiveRegion,
                hasThresholdExceedingFlash: hasThresholdExceedingFlash
            };
        }
        """;

    // At the narrow (320px) viewport, measures how much vertical space sticky/fixed elements
    // consume and whether a keyboard-focused element ends up covered by one of them.
    private const string StickyPassScript = """
        () => {
            function isVisible(el) {
                const style = window.getComputedStyle(el);
                if (style.display === 'none' || style.visibility === 'hidden' || parseFloat(style.opacity) === 0) return false;
                const rect = el.getBoundingClientRect();
                return rect.width > 0 && rect.height > 0;
            }

            function describe(el) {
                const name = (el.textContent || el.getAttribute('aria-label') || el.getAttribute('alt') || '').trim().slice(0, 40);
                return '<' + el.tagName.toLowerCase() + '> "' + name + '"';
            }

            const stickyEls = Array.from(document.querySelectorAll('*')).filter(el => {
                const style = window.getComputedStyle(el);
                return (style.position === 'sticky' || style.position === 'fixed') && isVisible(el);
            });

            let stickyHeight = 0;
            for (const el of stickyEls) {
                stickyHeight += el.getBoundingClientRect().height;
            }
            const stickyElementsConsumeExcessiveViewport = window.innerHeight > 0 && (stickyHeight / window.innerHeight) > 0.2;

            const obscuredElementDescriptions = [];
            const candidates = document.querySelectorAll('a[href], button, input:not([type="hidden"]), select, textarea, [tabindex]');
            for (const el of candidates) {
                if (!isVisible(el)) continue;
                el.focus({ preventScroll: true });
                const rect = el.getBoundingClientRect();
                let obscured = false;
                for (const sticky of stickyEls) {
                    if (sticky === el || sticky.contains(el)) continue;
                    const sRect = sticky.getBoundingClientRect();
                    const overlapsX = rect.left < sRect.right && rect.right > sRect.left;
                    const overlapsY = rect.top < sRect.bottom && rect.bottom > sRect.top;
                    if (overlapsX && overlapsY) { obscured = true; break; }
                }
                el.blur();
                if (obscured) obscuredElementDescriptions.push(describe(el));
            }

            return {
                stickyElementsConsumeExcessiveViewport: stickyElementsConsumeExcessiveViewport,
                obscuredElementDescriptions: obscuredElementDescriptions
            };
        }
        """;

    private sealed class MainPassResultDto
    {
        public RenderedTextStyleDto[] TextStyles { get; set; } = Array.Empty<RenderedTextStyleDto>();
        public RenderedElementDto[] Elements { get; set; } = Array.Empty<RenderedElementDto>();
        public string[] AnimatedElementDescriptions { get; set; } = Array.Empty<string>();
    }

    private sealed class StructuralPassResultDto
    {
        public bool FocusOrderDivergesFromVisualOrder { get; set; }
        public string[] FocusAppearanceBelowThreshold { get; set; } = Array.Empty<string>();
        public bool HasBypassMechanism { get; set; }
        public bool HasMainLandmark { get; set; }
        public string[] ContentOutsideAnyLandmark { get; set; } = Array.Empty<string>();
        public string[] DuplicateUnlabelledLandmarkDescriptions { get; set; } = Array.Empty<string>();
        public bool HeadingOrderDivergesFromVisualOrder { get; set; }
        public string[] DynamicStatusContentWithoutLiveRegion { get; set; } = Array.Empty<string>();
        public bool HasThresholdExceedingFlash { get; set; }
    }

    private sealed class StickyPassResultDto
    {
        public bool StickyElementsConsumeExcessiveViewport { get; set; }
        public string[] ObscuredElementDescriptions { get; set; } = Array.Empty<string>();
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
        public bool HasClickHandlerWithoutKeyboardAccess { get; set; }
        public string? VisibleLabelText { get; set; }
        public string? AccessibleName { get; set; }
        public bool IsDraggableWithoutAlternative { get; set; }
        public bool RequiresMultipointOrPathGesture { get; set; }
        public bool HasHoverOrFocusContentNotPersistent { get; set; }
        public bool HasInsufficientSpacingToNeighbor { get; set; }
        public bool IsObscuredWhenFocused { get; set; }
    }
}

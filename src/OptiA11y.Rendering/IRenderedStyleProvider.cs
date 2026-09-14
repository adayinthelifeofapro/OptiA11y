namespace OptiA11y.Rendering;

/// <summary>
/// Port for capturing the actual rendered/computed text styles of a page, as opposed to what
/// can be inferred from inline HTML attributes alone. Implementations render the page (e.g. via
/// a headless browser) and report the resolved color/background/font metrics for every visible
/// text node, so contrast and readability rules can evaluate what users actually see.
/// </summary>
public interface IRenderedStyleProvider
{
    /// <summary>
    /// Renders <paramref name="pageUrl"/> and returns the computed style of every visible text
    /// node found. Returns an empty list if the page could not be rendered (e.g. unreachable
    /// URL, browser unavailable) rather than throwing, so callers can treat this as a
    /// best-effort enrichment step.
    /// </summary>
    Task<IReadOnlyList<RenderedTextStyle>> CaptureAsync(Uri pageUrl, CancellationToken cancellationToken = default);
}

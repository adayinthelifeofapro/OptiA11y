namespace OptiA11y.Rendering;

/// <summary>
/// Port for capturing the actual rendered/computed diagnostics of a page, as opposed to what
/// can be inferred from inline HTML attributes alone. Implementations render the page (e.g. via
/// a headless browser) and report text styling, per-element interaction diagnostics, reflow, and
/// text-spacing tolerance, so the rendered-only rules can evaluate what users actually see and
/// experience.
/// </summary>
public interface IRenderedStyleProvider
{
    /// <summary>
    /// Renders <paramref name="pageUrl"/> and returns everything captured from it. Returns
    /// <see cref="RenderedPageDiagnostics.Empty"/> if the page could not be rendered (e.g.
    /// unreachable URL, browser unavailable) rather than throwing, so callers can treat this as
    /// a best-effort enrichment step.
    /// </summary>
    Task<RenderedPageDiagnostics> CaptureAsync(Uri pageUrl, CancellationToken cancellationToken = default);
}

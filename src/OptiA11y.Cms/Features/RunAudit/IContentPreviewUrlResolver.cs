namespace OptiA11y.Cms.Features.RunAudit;

/// <summary>
/// Port for resolving a public/preview URL for a content item, so the rendering slice can load
/// it in a headless browser to capture actual computed styles. Left as a host-specific interface
/// because URL resolution depends on the CMS 13 site's routing/host configuration; the default
/// registration is a no-op that returns null, which causes rendered-style enrichment to be
/// silently skipped (existing inline-style-only behavior is unaffected).
/// </summary>
public interface IContentPreviewUrlResolver
{
    /// <summary>
    /// Resolves a URL that renders <paramref name="contentReference"/> as end users would see it,
    /// or null if no such URL can be determined.
    /// </summary>
    Uri? ResolvePreviewUrl(string contentReference);
}

/// <summary>
/// Default no-op resolver. Hosts that want rendered-style enrichment (real contrast/readability
/// checks against actual computed CSS) must replace this registration with one that knows how to
/// build a preview URL for their site.
/// </summary>
public sealed class NullContentPreviewUrlResolver : IContentPreviewUrlResolver
{
    public Uri? ResolvePreviewUrl(string contentReference) => null;
}

namespace OptiA11y.Cms12.Features.RunAudit;

/// <summary>
/// Builds an <see cref="Core.Model.AuditDocument"/> from a PaaS <c>IContent</c> item.
/// Kept as a narrow port here so the handler doesn't depend on a concrete CMS content type,
/// which keeps this feature testable without a running CMS.
/// </summary>
public interface IContentAuditDocumentAdapter
{
    /// <summary>
    /// Builds the normalised audit document for the given content reference. Returns null
    /// if the content reference could not be resolved.
    /// </summary>
    Task<Core.Model.AuditDocument?> BuildAsync(string contentReference, CancellationToken cancellationToken);
}

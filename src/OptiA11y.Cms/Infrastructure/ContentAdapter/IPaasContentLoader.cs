namespace OptiA11y.Cms.Infrastructure.ContentAdapter;

/// <summary>
/// Loads a <see cref="PaasContentNode"/> for a given content reference. In a real PaaS host
/// this wraps <c>IContentLoader</c>; kept as an interface here so the mapping logic in
/// <see cref="PaasContentAdapter"/> is testable without a running CMS.
/// </summary>
public interface IPaasContentLoader
{
    Task<PaasContentNode?> LoadAsync(string contentReference, CancellationToken cancellationToken);
}

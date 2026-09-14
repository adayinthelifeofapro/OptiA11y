namespace OptiA11y.Saas.ContentApi;

/// <summary>
/// Fetches a <see cref="SaasContentResponse"/> for a content reference from the SaaS REST API.
/// Kept as an interface so the mapping logic in <see cref="SaasContentAdapter"/> is testable
/// without a network call, and so the HTTP client concern stays isolated from the mapping.
/// </summary>
public interface ISaasContentApiClient
{
    Task<SaasContentResponse?> GetContentAsync(string contentReference, CancellationToken cancellationToken);
}

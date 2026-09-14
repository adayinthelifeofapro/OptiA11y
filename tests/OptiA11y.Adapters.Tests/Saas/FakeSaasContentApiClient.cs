using OptiA11y.Saas.ContentApi;

namespace OptiA11y.Adapters.Tests.Saas;

internal sealed class FakeSaasContentApiClient : ISaasContentApiClient
{
    private readonly SaasContentResponse? _response;

    public FakeSaasContentApiClient(SaasContentResponse? response)
    {
        _response = response;
    }

    public Task<SaasContentResponse?> GetContentAsync(string contentReference, CancellationToken cancellationToken) =>
        Task.FromResult(_response);
}

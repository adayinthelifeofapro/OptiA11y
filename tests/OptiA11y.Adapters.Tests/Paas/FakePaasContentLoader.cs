using OptiA11y.Cms.Infrastructure.ContentAdapter;

namespace OptiA11y.Adapters.Tests.Paas;

internal sealed class FakePaasContentLoader : IPaasContentLoader
{
    private readonly PaasContentNode? _node;

    public FakePaasContentLoader(PaasContentNode? node)
    {
        _node = node;
    }

    public Task<PaasContentNode?> LoadAsync(string contentReference, CancellationToken cancellationToken) =>
        Task.FromResult(_node);
}

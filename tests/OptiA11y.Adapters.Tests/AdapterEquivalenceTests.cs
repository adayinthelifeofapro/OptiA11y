using OptiA11y.Adapters.Tests.Paas;
using OptiA11y.Adapters.Tests.Saas;
using OptiA11y.Cms.Infrastructure.ContentAdapter;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Saas.ContentApi;
using Xunit;

namespace OptiA11y.Adapters.Tests;

/// <summary>
/// Confirms the plan's "both-flavours" resolution actually holds: the same input content shape
/// produces equivalent fragments regardless of whether it arrives via the PaaS or SaaS adapter.
/// </summary>
public sealed class AdapterEquivalenceTests
{
    private const string Html = "<h1>Title</h1><img src='a.jpg' alt='' /><a href='/x'>click here</a>";

    [Fact]
    public async Task PaasAndSaasAdapters_ProduceEquivalentFragmentCounts_ForEquivalentInput()
    {
        var paasNode = new PaasContentNode(
            "page-1",
            BlockName: null,
            Properties: new[] { new PaasProperty("MainBody", PaasPropertyKind.XhtmlString, Html, Array.Empty<PaasContentNode>()) });

        var paasAdapter = new PaasContentAdapter(new FakePaasContentLoader(paasNode));
        var paasDocument = await paasAdapter.BuildAsync("page-1", CancellationToken.None);

        var saasResponse = new SaasContentResponse
        {
            ContentLink = "page-1",
            Properties = new List<SaasContentProperty>
            {
                new() { Name = "MainBody", PropertyDataType = "richText", Html = Html }
            }
        };

        var saasAdapter = new SaasContentAdapter(new FakeSaasContentApiClient(saasResponse));
        var saasDocument = await saasAdapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(paasDocument);
        Assert.NotNull(saasDocument);

        Assert.Equal(paasDocument!.Get<HeadingFragment>().Count(), saasDocument!.Get<HeadingFragment>().Count());
        Assert.Equal(paasDocument.Get<ImageFragment>().Count(), saasDocument.Get<ImageFragment>().Count());
        Assert.Equal(paasDocument.Get<LinkFragment>().Count(), saasDocument.Get<LinkFragment>().Count());

        var paasHeading = paasDocument.Get<HeadingFragment>().Single();
        var saasHeading = saasDocument.Get<HeadingFragment>().Single();
        Assert.Equal(paasHeading.Text, saasHeading.Text);
        Assert.Equal(paasHeading.Location.PropertyName, saasHeading.Location.PropertyName);
    }
}

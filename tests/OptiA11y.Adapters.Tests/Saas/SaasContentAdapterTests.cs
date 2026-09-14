using OptiA11y.Core.Model.Fragments;
using OptiA11y.Saas.ContentApi;
using Xunit;

namespace OptiA11y.Adapters.Tests.Saas;

public sealed class SaasContentAdapterTests
{
    [Fact]
    public async Task BuildAsync_ReturnsNull_WhenContentNotFound()
    {
        var adapter = new SaasContentAdapter(new FakeSaasContentApiClient(null));

        var result = await adapter.BuildAsync("missing", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task BuildAsync_ExtractsFragmentsFromTopLevelHtmlProperty()
    {
        var response = new SaasContentResponse
        {
            ContentLink = "page-1",
            Properties = new List<SaasContentProperty>
            {
                new()
                {
                    Name = "MainBody",
                    PropertyDataType = "richText",
                    Html = "<h1>Title</h1><img src='a.jpg' alt='' /><a href='/x'>click here</a>"
                }
            }
        };

        var adapter = new SaasContentAdapter(new FakeSaasContentApiClient(response));
        var document = await adapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(document);
        Assert.Single(document!.Get<HeadingFragment>());
        Assert.Single(document.Get<ImageFragment>());
        Assert.Single(document.Get<LinkFragment>());
    }

    [Fact]
    public async Task BuildAsync_RecursesIntoNestedContentAreas_AndRecordsBlockPath()
    {
        var nestedBlock = new SaasContentResponse
        {
            ContentLink = "block-1",
            Properties = new List<SaasContentProperty>
            {
                new() { Name = "Body", PropertyDataType = "richText", Html = "<img src='nested.jpg' />" }
            }
        };

        var root = new SaasContentResponse
        {
            ContentLink = "page-1",
            Properties = new List<SaasContentProperty>
            {
                new()
                {
                    Name = "MainContentArea",
                    PropertyDataType = "contentArea",
                    ContentArea = new List<SaasContentResponse> { nestedBlock }
                }
            }
        };

        var adapter = new SaasContentAdapter(new FakeSaasContentApiClient(root));
        var document = await adapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(document);
        var image = document!.Get<ImageFragment>().Single();

        Assert.Equal("block-1", image.Location.ContentReference);
        Assert.Equal("Body", image.Location.PropertyName);
        Assert.Equal(new[] { "MainContentArea:block-1" }, image.Location.BlockPath);
    }

    [Fact]
    public async Task BuildAsync_ExtractsRootDisplayName_AsPageMetadataFragment()
    {
        var response = new SaasContentResponse
        {
            ContentLink = "page-1",
            DisplayName = "Contact us",
            Properties = new List<SaasContentProperty>()
        };

        var adapter = new SaasContentAdapter(new FakeSaasContentApiClient(response));
        var document = await adapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(document);
        var metadata = document!.Get<PageMetadataFragment>().Single();
        Assert.Equal("Contact us", metadata.DisplayName);
    }
}

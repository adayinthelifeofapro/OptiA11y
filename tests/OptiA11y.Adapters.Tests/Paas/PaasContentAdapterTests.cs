using OptiA11y.Cms.Infrastructure.ContentAdapter;
using OptiA11y.Core.Model.Fragments;
using Xunit;

namespace OptiA11y.Adapters.Tests.Paas;

public sealed class PaasContentAdapterTests
{
    [Fact]
    public async Task BuildAsync_ReturnsNull_WhenContentNotFound()
    {
        var adapter = new PaasContentAdapter(new FakePaasContentLoader(null));

        var result = await adapter.BuildAsync("missing", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task BuildAsync_ExtractsFragmentsFromTopLevelXhtmlProperty()
    {
        var node = new PaasContentNode(
            "page-1",
            BlockName: null,
            Properties: new[]
            {
                new PaasProperty(
                    "MainBody",
                    PaasPropertyKind.XhtmlString,
                    "<h1>Title</h1><img src='a.jpg' alt='' /><a href='/x'>click here</a>",
                    Array.Empty<PaasContentNode>())
            });

        var adapter = new PaasContentAdapter(new FakePaasContentLoader(node));
        var document = await adapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(document);
        Assert.Single(document!.Get<HeadingFragment>());
        Assert.Single(document.Get<ImageFragment>());
        Assert.Single(document.Get<LinkFragment>());

        var heading = document.Get<HeadingFragment>().Single();
        Assert.Equal("page-1", heading.Location.ContentReference);
        Assert.Equal("MainBody", heading.Location.PropertyName);
        Assert.Empty(heading.Location.BlockPath);
    }

    [Fact]
    public async Task BuildAsync_RecursesIntoNestedBlocks_AndRecordsBlockPath()
    {
        var nestedBlock = new PaasContentNode(
            "block-1",
            BlockName: "TeaserBlock",
            Properties: new[]
            {
                new PaasProperty(
                    "Body",
                    PaasPropertyKind.XhtmlString,
                    "<img src='nested.jpg' />",
                    Array.Empty<PaasContentNode>())
            });

        var root = new PaasContentNode(
            "page-1",
            BlockName: null,
            Properties: new[]
            {
                new PaasProperty(
                    "MainContentArea",
                    PaasPropertyKind.ContentArea,
                    Html: null,
                    Blocks: new[] { nestedBlock })
            });

        var adapter = new PaasContentAdapter(new FakePaasContentLoader(root));
        var document = await adapter.BuildAsync("page-1", CancellationToken.None);

        Assert.NotNull(document);
        var image = document!.Get<ImageFragment>().Single();

        Assert.Equal("block-1", image.Location.ContentReference);
        Assert.Equal("Body", image.Location.PropertyName);
        Assert.Equal(new[] { "MainContentArea:TeaserBlock" }, image.Location.BlockPath);
        Assert.Null(image.AltText);
    }
}

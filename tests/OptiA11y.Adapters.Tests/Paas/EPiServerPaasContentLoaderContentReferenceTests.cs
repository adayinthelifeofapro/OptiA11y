using EPiServer.Core;
using OptiA11y.Cms.Infrastructure.ContentAdapter;
using Xunit;

namespace OptiA11y.Adapters.Tests.Paas;

public sealed class EPiServerPaasContentLoaderContentReferenceTests
{
    [Theory]
    [InlineData("7", 7)]
    [InlineData("7_8", 7)]
    [InlineData("7_8__CatchAll", 7)]
    public void TryParseContentReference_ValidShapes_ResolvesExpectedId(string input, int expectedId)
    {
        var success = EPiServerPaasContentLoader.TryParseContentReference(input, out var reference);

        Assert.True(success);
        Assert.Equal(expectedId, reference.ID);
    }

    [Fact]
    public void TryParseContentReference_UrlEncodedValue_IsUnescapedBeforeParsing()
    {
        var success = EPiServerPaasContentLoader.TryParseContentReference("7%5F8", out var reference);

        Assert.True(success);
        Assert.Equal(7, reference.ID);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("not-a-content-reference")]
    public void TryParseContentReference_InvalidValues_ReturnsFalse(string? input)
    {
        var success = EPiServerPaasContentLoader.TryParseContentReference(input, out var reference);

        Assert.False(success);
        Assert.Equal(ContentReference.EmptyReference, reference);
    }
}

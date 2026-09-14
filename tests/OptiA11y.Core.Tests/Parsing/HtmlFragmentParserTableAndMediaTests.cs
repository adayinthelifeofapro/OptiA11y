using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;
using Xunit;

namespace OptiA11y.Core.Tests.Parsing;

public sealed class HtmlFragmentParserTableAndMediaTests
{
    private static readonly SourceLocation BaseLocation = SourceLocation.OnProperty("content-1", "MainBody");

    [Fact]
    public void Table_WithHeaderRowAndCaption_IsCapturedCorrectly()
    {
        const string html = "<table><caption>Prices</caption><tr><th>Item</th><th>Price</th></tr><tr><td>Widget</td><td>£1</td></tr></table>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var table = Assert.Single(fragments.OfType<TableFragment>());

        Assert.True(table.HasHeaderRow);
        Assert.True(table.HasCaption);
        Assert.Equal(2, table.ColumnCount);
    }

    [Fact]
    public void Table_WithoutHeaderRowOrCaption_IsCapturedCorrectly()
    {
        const string html = "<table><tr><td>Widget</td><td>£1</td></tr></table>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var table = Assert.Single(fragments.OfType<TableFragment>());

        Assert.False(table.HasHeaderRow);
        Assert.False(table.HasCaption);
    }

    [Fact]
    public void Video_WithCaptionsTrack_HasCaptionsOrTranscriptTrue()
    {
        const string html = "<video src=\"clip.mp4\"><track kind=\"captions\" src=\"captions.vtt\" /></video>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var media = Assert.Single(fragments.OfType<MediaFragment>());

        Assert.Equal("video", media.MediaKind);
        Assert.True(media.HasCaptionsOrTranscript);
    }

    [Fact]
    public void Video_WithoutTrackOrTranscript_HasCaptionsOrTranscriptFalse()
    {
        const string html = "<div><video src=\"clip.mp4\"></video></div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var media = Assert.Single(fragments.OfType<MediaFragment>());

        Assert.False(media.HasCaptionsOrTranscript);
    }

    [Fact]
    public void Audio_WithNearbyTranscriptLink_HasCaptionsOrTranscriptTrue()
    {
        const string html = "<div><audio src=\"clip.mp3\"></audio><a href=\"/transcript\">Read the transcript</a></div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var media = Assert.Single(fragments.OfType<MediaFragment>());

        Assert.Equal("audio", media.MediaKind);
        Assert.True(media.HasCaptionsOrTranscript);
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Rendering;

namespace OptiA11y.Rendering.Tests;

public sealed class RenderedStyleFragmentBuilderTests
{
    private static readonly SourceLocation Location = SourceLocation.OnProperty("content-1", "__RenderedStyles");

    [Fact]
    public void Build_LowContrastText_ProducesColorContrastFragment()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Low contrast text", "rgb(119, 119, 119)", "rgb(102, 102, 102)", 16, "400", "left")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        var contrast = Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.True(contrast.ContrastRatio < 4.5);
        Assert.False(contrast.IsLargeText);
    }

    [Fact]
    public void Build_BoldLargeText_IsFlaggedAsLargeText()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Bold large text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 20, "700", "left")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        var contrast = Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.True(contrast.IsLargeText);
    }

    [Fact]
    public void Build_JustifiedText_ProducesTextStyleFragment()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Justified text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 16, "400", "justify")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        var textStyle = Assert.Single(fragments.OfType<TextStyleFragment>());
        Assert.Equal("justify", textStyle.TextAlign);
    }

    [Fact]
    public void Build_TinyText_ProducesTextStyleFragment()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Tiny text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 9, "400", "left")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        var textStyle = Assert.Single(fragments.OfType<TextStyleFragment>());
        Assert.Equal(9, textStyle.FontSizePx);
    }

    [Fact]
    public void Build_NormalText_ProducesNoFragments()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Normal readable text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 16, "400", "left")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.Empty(fragments.OfType<TextStyleFragment>());
    }

    [Fact]
    public void Build_UnparseableColors_SkipsContrastFragment()
    {
        var styles = new[]
        {
            new RenderedTextStyle("Text with weird color", "not-a-color", "also-not-a-color", 16, "400", "left")
        };

        var fragments = RenderedStyleFragmentBuilder.Build(styles, Location);

        Assert.Empty(fragments);
    }
}

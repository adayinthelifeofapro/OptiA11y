using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Rendering;

namespace OptiA11y.Rendering.Tests;

public sealed class RenderedStyleFragmentBuilderTests
{
    private static readonly SourceLocation Location = SourceLocation.OnProperty("content-1", "__RenderedStyles");

    private static RenderedPageDiagnostics WithTextStyles(params RenderedTextStyle[] styles) =>
        RenderedPageDiagnostics.Empty with { TextStyles = styles };

    [Fact]
    public void Build_LowContrastText_ProducesColorContrastFragment()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Low contrast text", "rgb(119, 119, 119)", "rgb(102, 102, 102)", 16, "400", "left"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var contrast = Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.True(contrast.ContrastRatio < 4.5);
        Assert.False(contrast.IsLargeText);
    }

    [Fact]
    public void Build_BoldLargeText_IsFlaggedAsLargeText()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Bold large text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 20, "700", "left"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var contrast = Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.True(contrast.IsLargeText);
    }

    [Fact]
    public void Build_JustifiedText_ProducesTextStyleFragment()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Justified text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 16, "400", "justify"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var textStyle = Assert.Single(fragments.OfType<TextStyleFragment>());
        Assert.Equal("justify", textStyle.TextAlign);
    }

    [Fact]
    public void Build_TinyText_ProducesTextStyleFragment()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Tiny text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 9, "400", "left"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var textStyle = Assert.Single(fragments.OfType<TextStyleFragment>());
        Assert.Equal(9, textStyle.FontSizePx);
    }

    [Fact]
    public void Build_NormalText_ProducesNoFragments()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Normal readable text", "rgb(0, 0, 0)", "rgb(255, 255, 255)", 16, "400", "left"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.Empty(fragments.OfType<TextStyleFragment>());
    }

    [Fact]
    public void Build_UnparseableColors_SkipsContrastFragment()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Text with weird color", "not-a-color", "also-not-a-color", 16, "400", "left"));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments);
    }

    [Fact]
    public void Build_ImageBackground_MarksContrastFragmentAsBackgroundIsImage()
    {
        var diagnostics = WithTextStyles(
            new RenderedTextStyle("Text over a photo", "rgb(119, 119, 119)", "rgb(102, 102, 102)", 16, "400", "left", HasBackgroundImage: true));

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var contrast = Assert.Single(fragments.OfType<ColorContrastFragment>());
        Assert.True(contrast.BackgroundIsImage);
    }

    [Fact]
    public void Build_UndersizedElement_ProducesTargetSizeFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<button> \"Close\"", 18, 18, true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var target = Assert.Single(fragments.OfType<TargetSizeFragment>());
        Assert.Equal(18, target.WidthPx);
    }

    [Fact]
    public void Build_SufficientlySizedElement_ProducesNoTargetSizeFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<button> \"Close\"", 44, 44, true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments.OfType<TargetSizeFragment>());
    }

    [Fact]
    public void Build_MissingFocusIndicator_ProducesFocusIndicatorFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<a> \"Skip to content\"", 100, 24, false) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<FocusIndicatorFragment>());
    }

    [Fact]
    public void Build_PresentFocusIndicator_ProducesNoFocusIndicatorFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<a> \"Skip to content\"", 100, 24, true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments.OfType<FocusIndicatorFragment>());
    }

    [Fact]
    public void Build_AnimatedElement_ProducesMotionFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            AnimatedElementDescriptions = new[] { "<div> \"\"" }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<MotionFragment>());
    }

    [Fact]
    public void Build_OverflowingAtNarrowViewport_ProducesReflowFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { OverflowsAtNarrowViewport = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<ReflowFragment>());
    }

    [Fact]
    public void Build_NoOverflowAtNarrowViewport_ProducesNoReflowFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { OverflowsAtNarrowViewport = false };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments.OfType<ReflowFragment>());
    }

    [Fact]
    public void Build_ClippedTextSpacingSample_ProducesTextSpacingFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            TextSpacingClippedSamples = new[] { "Sign up for our newsletter" }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var textSpacing = Assert.Single(fragments.OfType<TextSpacingFragment>());
        Assert.Equal("Sign up for our newsletter", textSpacing.SampleText);
    }
}

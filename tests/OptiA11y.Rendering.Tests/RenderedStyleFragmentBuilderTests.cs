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

    [Fact]
    public void Build_KeyboardTrap_ProducesKeyboardTrapFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HasKeyboardTrap = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<KeyboardTrapFragment>());
    }

    [Fact]
    public void Build_FocusOrderDiverges_ProducesFocusOrderFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { FocusOrderDivergesFromVisualOrder = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<FocusOrderFragment>());
    }

    [Fact]
    public void Build_FocusAppearanceBelowThreshold_ProducesFocusAppearanceFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            FocusAppearanceBelowThreshold = new[] { "<button> \"Submit\"" }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<FocusAppearanceFragment>());
    }

    [Fact]
    public void Build_OrientationLockOverflow_ProducesOrientationLockFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { OverflowsOrLosesContentUnderOrientationLock = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<OrientationLockFragment>());
    }

    [Fact]
    public void Build_LosesContentAtTextZoom_ProducesResizeTextFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { LosesContentAtTextZoom = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<ResizeTextFragment>());
    }

    [Fact]
    public void Build_StickyElementsConsumeExcessiveViewport_ProducesStickyObstructionFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { StickyElementsConsumeExcessiveViewport = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<StickyObstructionFragment>());
    }

    [Fact]
    public void Build_ThresholdExceedingFlash_ProducesFlashThresholdFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HasThresholdExceedingFlash = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<FlashThresholdFragment>());
    }

    [Fact]
    public void Build_DynamicStatusContentWithoutLiveRegion_ProducesStatusMessagesFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            DynamicStatusContentWithoutLiveRegion = new[] { "<div> \"Saved\"" }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<StatusMessagesFragment>());
    }

    [Fact]
    public void Build_NoBypassMechanism_ProducesBypassBlocksFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HasBypassMechanism = false };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<BypassBlocksFragment>());
    }

    [Fact]
    public void Build_HasBypassMechanism_ProducesNoBypassBlocksFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HasBypassMechanism = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments.OfType<BypassBlocksFragment>());
    }

    [Fact]
    public void Build_MissingMainLandmark_ProducesLandmarkCompletenessFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HasMainLandmark = false };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        var landmark = Assert.Single(fragments.OfType<LandmarkCompletenessFragment>());
        Assert.False(landmark.HasMainLandmark);
    }

    [Fact]
    public void Build_HeadingOrderDiverges_ProducesHeadingInViewportOrderFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with { HeadingOrderDivergesFromVisualOrder = true };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<HeadingInViewportOrderFragment>());
    }

    [Fact]
    public void Build_ElementWithClickHandlerWithoutKeyboardAccess_ProducesKeyboardOperableFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<div> \"Click me\"", 100, 40, true, HasClickHandlerWithoutKeyboardAccess: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<KeyboardOperableFragment>());
    }

    [Fact]
    public void Build_VisibleLabelNotInAccessibleName_ProducesLabelInNameFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[]
            {
                new RenderedElementDiagnostics(
                    "<button> \"Search\"",
                    100,
                    40,
                    true,
                    VisibleLabelText: "Search",
                    AccessibleName: "Go")
            }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<LabelInNameFragment>());
    }

    [Fact]
    public void Build_VisibleLabelContainedInAccessibleName_ProducesNoLabelInNameFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[]
            {
                new RenderedElementDiagnostics(
                    "<button> \"Search\"",
                    100,
                    40,
                    true,
                    VisibleLabelText: "Search",
                    AccessibleName: "Search the site")
            }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Empty(fragments.OfType<LabelInNameFragment>());
    }

    [Fact]
    public void Build_DraggableWithoutAlternative_ProducesDraggingMovementsFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<div> \"Slider\"", 100, 40, true, IsDraggableWithoutAlternative: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<DraggingMovementsFragment>());
    }

    [Fact]
    public void Build_RequiresMultipointOrPathGesture_ProducesPointerGesturesFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<div> \"Gallery\"", 100, 40, true, RequiresMultipointOrPathGesture: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<PointerGesturesFragment>());
    }

    [Fact]
    public void Build_HoverFocusContentNotPersistent_ProducesHoverFocusContentFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<a> \"Info\"", 100, 40, true, HasHoverOrFocusContentNotPersistent: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<HoverFocusContentFragment>());
    }

    [Fact]
    public void Build_InsufficientSpacingToNeighbor_ProducesPointerTargetSpacingFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<button> \"X\"", 18, 18, true, HasInsufficientSpacingToNeighbor: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<PointerTargetSpacingFragment>());
    }

    [Fact]
    public void Build_ObscuredWhenFocused_ProducesFocusNotObscuredFragment()
    {
        var diagnostics = RenderedPageDiagnostics.Empty with
        {
            Elements = new[] { new RenderedElementDiagnostics("<a> \"Menu\"", 100, 40, true, IsObscuredWhenFocused: true) }
        };

        var fragments = RenderedStyleFragmentBuilder.Build(diagnostics, Location);

        Assert.Single(fragments.OfType<FocusNotObscuredFragment>());
    }
}

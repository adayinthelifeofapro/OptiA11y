using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;

namespace OptiA11y.Rendering;

public static class RenderedStyleFragmentBuilder
{
    private const double LargeTextNormalWeightPx = 24;
    private const double LargeTextBoldWeightPx = 18.66;
    private const double MinimumTargetSizePx = 24;

    public static IReadOnlyList<ContentFragment> Build(
        RenderedPageDiagnostics diagnostics,
        SourceLocation location,
        int startingOrdinal = 0)
    {
        var fragments = new List<ContentFragment>();
        var ordinal = startingOrdinal;

        foreach (var style in diagnostics.TextStyles)
        {
            if (ColorContrastCalculator.TryParseColor(style.Color, out var fg)
                && ColorContrastCalculator.TryParseColor(style.BackgroundColor, out var bg))
            {
                var isBold = IsBold(style.FontWeight);
                var isLargeText = style.FontSizePx >= LargeTextNormalWeightPx
                    || (style.FontSizePx >= LargeTextBoldWeightPx && isBold);

                var ratio = ColorContrastCalculator.ComputeContrastRatio(fg, bg);

                fragments.Add(new ColorContrastFragment(
                    location with { Ordinal = ordinal++ },
                    style.Color,
                    style.BackgroundColor,
                    ratio,
                    isLargeText,
                    Truncate(style.Text),
                    style.HasBackgroundImage));
            }

            var isJustified = string.Equals(style.TextAlign, "justify", StringComparison.OrdinalIgnoreCase);
            var isTiny = style.FontSizePx is > 0 and < 12;

            if (isJustified || isTiny)
            {
                fragments.Add(new TextStyleFragment(
                    location with { Ordinal = ordinal++ },
                    style.TextAlign,
                    style.FontSizePx > 0 ? style.FontSizePx : null,
                    Truncate(style.Text)));
            }
        }

        foreach (var element in diagnostics.Elements)
        {
            if (element.WidthPx < MinimumTargetSizePx || element.HeightPx < MinimumTargetSizePx)
            {
                fragments.Add(new TargetSizeFragment(
                    location with { Ordinal = ordinal++ },
                    element.Description,
                    element.WidthPx,
                    element.HeightPx));
            }

            if (!element.HasVisibleFocusIndicator)
            {
                fragments.Add(new FocusIndicatorFragment(
                    location with { Ordinal = ordinal++ },
                    element.Description));
            }
        }

        foreach (var description in diagnostics.AnimatedElementDescriptions)
        {
            fragments.Add(new MotionFragment(location with { Ordinal = ordinal++ }, description));
        }

        if (diagnostics.OverflowsAtNarrowViewport)
        {
            fragments.Add(new ReflowFragment(location with { Ordinal = ordinal++ }));
        }

        foreach (var sample in diagnostics.TextSpacingClippedSamples)
        {
            fragments.Add(new TextSpacingFragment(location with { Ordinal = ordinal++ }, Truncate(sample)));
        }

        return fragments;
    }

    private static bool IsBold(string fontWeight)
    {
        if (string.Equals(fontWeight, "bold", StringComparison.OrdinalIgnoreCase)
            || string.Equals(fontWeight, "bolder", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return int.TryParse(fontWeight, out var numericWeight) && numericWeight >= 700;
    }

    private static string Truncate(string text) => text.Length > 60 ? text[..60] : text;
}

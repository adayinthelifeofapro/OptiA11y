using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;

namespace OptiA11y.Rendering;

/// <summary>
/// Converts <see cref="RenderedTextStyle"/> entries captured by an <see cref="IRenderedStyleProvider"/>
/// into the same <see cref="ColorContrastFragment"/>/<see cref="TextStyleFragment"/> shapes that
/// <c>HtmlFragmentParser</c> produces from inline styles, so the existing
/// <c>ColorContrastRule</c>/<c>TextReadabilityRule</c> can evaluate rendered, real-world styles
/// without any changes to the rule layer. This is a pure function - no browser dependency - kept
/// in <c>OptiA11y.Rendering</c> (rather than Core) because it consumes <see cref="RenderedTextStyle"/>,
/// which Core has no reason to know about.
/// </summary>
public static class RenderedStyleFragmentBuilder
{
    private const double LargeTextNormalWeightPx = 24;
    private const double LargeTextBoldWeightPx = 18.66;

    /// <summary>
    /// Builds contrast and text-style fragments from rendered text styles. <paramref name="location"/>
    /// is used as the base location for every fragment produced; ordinals are assigned sequentially
    /// starting at <paramref name="startingOrdinal"/> so callers can append these after any fragments
    /// already produced for the same content item.
    /// </summary>
    public static IReadOnlyList<ContentFragment> Build(
        IReadOnlyList<RenderedTextStyle> renderedStyles,
        SourceLocation location,
        int startingOrdinal = 0)
    {
        var fragments = new List<ContentFragment>();
        var ordinal = startingOrdinal;

        foreach (var style in renderedStyles)
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
                    Truncate(style.Text)));
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

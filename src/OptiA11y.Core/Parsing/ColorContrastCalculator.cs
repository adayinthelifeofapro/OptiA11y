using System.Globalization;

namespace OptiA11y.Core.Parsing;

/// <summary>
/// Parses CSS color values (hex, rgb/rgba) and computes the WCAG relative-luminance contrast
/// ratio between two colors, per the 1.4.3 contrast (minimum) formula. This is a pure,
/// deterministic calculation - no CMS or DOM dependency - so it belongs in Core alongside
/// <see cref="HtmlFragmentParser"/>.
/// </summary>
public static class ColorContrastCalculator
{
    /// <summary>
    /// Attempts to parse a CSS color string (e.g. "#ffffff", "#fff", "rgb(0,0,0)",
    /// "rgba(0,0,0,0.5)") into its (r, g, b) components. Named CSS colors are not supported.
    /// </summary>
    public static bool TryParseColor(string? value, out (byte R, byte G, byte B) color)
    {
        color = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();

        if (value.StartsWith('#'))
        {
            var hex = value[1..];
            if (hex.Length == 3)
            {
                hex = string.Concat(hex[0], hex[0], hex[1], hex[1], hex[2], hex[2]);
            }

            if (hex.Length != 6 || !byte.TryParse(hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r)
                || !byte.TryParse(hex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g)
                || !byte.TryParse(hex[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
            {
                return false;
            }

            color = (r, g, b);
            return true;
        }

        if (value.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
        {
            var start = value.IndexOf('(');
            var end = value.IndexOf(')');
            if (start < 0 || end < 0 || end <= start)
            {
                return false;
            }

            var parts = value[(start + 1)..end].Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 3
                || !byte.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var r)
                || !byte.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var g)
                || !byte.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var b))
            {
                return false;
            }

            color = (r, g, b);
            return true;
        }

        return false;
    }

    /// <summary>Computes the WCAG contrast ratio between two colors (a value between 1 and 21).</summary>
    public static double ComputeContrastRatio((byte R, byte G, byte B) a, (byte R, byte G, byte B) b)
    {
        var luminanceA = RelativeLuminance(a) + 0.05;
        var luminanceB = RelativeLuminance(b) + 0.05;
        return luminanceA > luminanceB ? luminanceA / luminanceB : luminanceB / luminanceA;
    }

    private static double RelativeLuminance((byte R, byte G, byte B) color)
    {
        double Channel(byte value)
        {
            var normalized = value / 255.0;
            return normalized <= 0.03928
                ? normalized / 12.92
                : Math.Pow((normalized + 0.055) / 1.055, 2.4);
        }

        return 0.2126 * Channel(color.R) + 0.7152 * Channel(color.G) + 0.0722 * Channel(color.B);
    }
}

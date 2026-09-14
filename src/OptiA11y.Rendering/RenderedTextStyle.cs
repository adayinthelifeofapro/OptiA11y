namespace OptiA11y.Rendering;

/// <summary>
/// The computed style of a single visible text node, extracted from a rendered page via a
/// headless browser. This captures what the browser actually paints, in contrast to
/// <c>HtmlFragmentParser</c>'s inline-<c>style</c>-only view, which misses anything driven by
/// external stylesheets, CSS classes, or inherited/theme styles.
/// </summary>
/// <param name="Text">The trimmed, visible text content of the node, used to correlate back to Core fragments.</param>
/// <param name="Color">The computed foreground color, as an rgb()/rgba() string.</param>
/// <param name="BackgroundColor">
/// The computed effective background color behind this text (resolved by walking up the
/// ancestor chain until an opaque background is found), as an rgb()/rgba() string.
/// </param>
/// <param name="FontSizePx">The computed font size in pixels.</param>
/// <param name="FontWeight">The computed font weight (e.g. "400", "700", "bold").</param>
/// <param name="TextAlign">The computed text-align value (e.g. "left", "justify").</param>
/// <param name="HasBackgroundImage">
/// True when the nearest ancestor with a paintable background actually paints an image or
/// gradient rather than a flat color, meaning <see cref="BackgroundColor"/> is only the fallback
/// solid color behind it, not what's actually visible.
/// </param>
public sealed record RenderedTextStyle(
    string Text,
    string Color,
    string BackgroundColor,
    double FontSizePx,
    string FontWeight,
    string TextAlign,
    bool HasBackgroundImage = false);

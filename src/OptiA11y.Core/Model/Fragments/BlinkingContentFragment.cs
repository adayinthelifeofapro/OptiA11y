namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// An element whose markup declares blinking via the deprecated &lt;blink&gt;/&lt;marquee&gt;
/// tags or an inline <c>text-decoration: blink</c>/<c>animation</c> style naming "blink" -
/// content that starts blinking automatically with no way to stop it (WCAG 2.2.2). This is a
/// deterministic structural fact about the markup, so the corresponding rule reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element, for editor-facing messages.</param>
public sealed record BlinkingContentFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

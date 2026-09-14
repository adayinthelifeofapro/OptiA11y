namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Non-text content that isn't covered by <see cref="ImageFragment"/>: inline SVG, image-map
/// &lt;area&gt; elements, and &lt;object&gt;/&lt;embed&gt; embeds. Each of these needs its own
/// accessible-name computation, so they get their own fragment rather than being folded into
/// <see cref="ImageFragment"/>.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementType">"svg", "area", "object", or "embed".</param>
/// <param name="HasAccessibleName">
/// True when the element has a mechanism to expose an accessible name: for &lt;svg&gt;, a
/// &lt;title&gt; child or aria-label/aria-labelledby; for &lt;area&gt;, a non-empty alt or
/// aria-label; for &lt;object&gt;/&lt;embed&gt;, a title/aria-label or non-empty fallback content.
/// </param>
/// <param name="IsAriaHidden">True when the element is explicitly hidden from assistive technology, which is a deliberate and valid way to mark it as decorative.</param>
public sealed record NonTextElementFragment(
    SourceLocation Location,
    string ElementType,
    bool HasAccessibleName,
    bool IsAriaHidden) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The element's tag name.</param>
/// <param name="TabIndex">The parsed <c>tabindex</c> attribute value, or null if absent/unparseable.</param>
/// <param name="AriaHidden">True when the element has <c>aria-hidden="true"</c>.</param>
/// <param name="Role">The element's explicit <c>role</c> attribute value, or null if absent.</param>
/// <param name="AccessKey">The element's <c>accesskey</c> attribute value, or null if absent.</param>
/// <param name="IsNativelyInteractive">
/// True when the element is natively focusable/interactive (a link with an href, a button, or a
/// form control), used to detect elements that are both interactive and hidden from assistive
/// technology.
/// </param>
public sealed record InteractiveAttributesFragment(
    SourceLocation Location,
    string TagName,
    int? TabIndex,
    bool AriaHidden,
    string? Role,
    string? AccessKey,
    bool IsNativelyInteractive) : ContentFragment(Location);

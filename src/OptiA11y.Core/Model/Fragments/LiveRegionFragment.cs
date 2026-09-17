namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Captures a live-region element - one with an explicit <c>aria-live</c> attribute, or a role
/// that implies live-region behaviour (<c>status</c>, <c>alert</c>, <c>log</c>, <c>marquee</c>,
/// <c>timer</c>). Used to flag an invalid <c>aria-live</c> politeness value (deterministic), and
/// to flag live-region markup that looks like it was applied to static, one-time content rather
/// than genuinely dynamic content (a judgement call, since the parser cannot observe runtime
/// updates).
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="TagName">The element's tag name.</param>
/// <param name="Role">The element's role, if the live-region behaviour comes from a role rather than <c>aria-live</c>.</param>
/// <param name="AriaLiveValue">The raw <c>aria-live</c> attribute value, or null if absent.</param>
/// <param name="HasInvalidPolitenessValue">True when <see cref="AriaLiveValue"/> is set but isn't one of "off", "polite", or "assertive".</param>
/// <param name="TextLength">The trimmed length of the element's own text content, used as a weak signal for whether this looks like a static caption/label rather than a dynamic status area.</param>
public sealed record LiveRegionFragment(
    SourceLocation Location,
    string TagName,
    string? Role,
    string? AriaLiveValue,
    bool HasInvalidPolitenessValue,
    int TextLength) : ContentFragment(Location);

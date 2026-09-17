namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Marks content that was inserted into the page dynamically (via script, after load) into an
/// element lacking any live-region semantics (<c>aria-live</c>, <c>role="status"</c>, or
/// <c>role="alert"</c>) (WCAG 4.1.3), from the rendered-style enrichment slice. Whether the
/// specific content genuinely needs to be announced is a judgement call.
/// </summary>
/// <param name="Location">Where this element was found.</param>
/// <param name="ElementDescription">A short human-readable description of the element.</param>
public sealed record StatusMessagesFragment(
    SourceLocation Location,
    string ElementDescription) : ContentFragment(Location);

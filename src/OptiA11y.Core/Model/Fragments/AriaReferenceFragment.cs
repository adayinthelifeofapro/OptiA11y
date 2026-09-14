namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// One id reference made by an <c>aria-labelledby</c>, <c>aria-describedby</c>, or a
/// &lt;label for&gt; attribute. Resolution is scoped to the single HTML fragment being parsed
/// (one property's markup), so an unresolved reference is NOT proof of a broken page - the
/// target could legitimately live in the page template or a sibling property. That ambiguity is
/// why the corresponding rule reports NeedsReview rather than Fail.
/// </summary>
/// <param name="Location">Where the referencing element was found.</param>
/// <param name="SourceAttribute">"aria-labelledby", "aria-describedby", or "for".</param>
/// <param name="ReferencedId">The id token being referenced.</param>
/// <param name="ResolvedWithinSameFragment">True when an element with this id was found within the same parsed HTML fragment.</param>
public sealed record AriaReferenceFragment(
    SourceLocation Location,
    string SourceAttribute,
    string ReferencedId,
    bool ResolvedWithinSameFragment) : ContentFragment(Location);

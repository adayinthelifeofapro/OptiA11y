namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// An in-page navigation link (<c>href="#id"</c>) and whether its target could be resolved
/// within the same parsed HTML fragment. Resolution is scoped to a single property's markup, so
/// an unresolved target is NOT proof of a broken link - the target could legitimately live in the
/// page template or a sibling property this rule cannot see. That ambiguity is why the
/// corresponding rule reports <see cref="Model.Confidence.NeedsReview"/> rather than
/// <see cref="Model.Confidence.Fail"/>.
/// </summary>
/// <param name="Location">Where the referencing link was found.</param>
/// <param name="TargetId">The id token being referenced (the part after '#').</param>
/// <param name="ResolvedWithinSameFragment">True when an element with this id was found within the same parsed HTML fragment.</param>
public sealed record SkipLinkFragment(
    SourceLocation Location,
    string TargetId,
    bool ResolvedWithinSameFragment) : ContentFragment(Location);

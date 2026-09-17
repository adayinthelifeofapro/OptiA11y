namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this definition list was found.</param>
/// <param name="HasOrphanedTerm">True when a `&lt;dt&gt;` has no following `&lt;dd&gt;` before the next `&lt;dt&gt;`/end of list.</param>
/// <param name="DescriptionBeforeTerm">True when a `&lt;dd&gt;` appears before any `&lt;dt&gt;` in the list.</param>
public sealed record DefinitionListFragment(
    SourceLocation Location,
    bool HasOrphanedTerm,
    bool DescriptionBeforeTerm) : ContentFragment(Location);

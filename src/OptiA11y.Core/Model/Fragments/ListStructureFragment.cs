namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this list-style structure was found.</param>
/// <param name="Kind">Either "list" for a semantic &lt;ul&gt;/&lt;ol&gt;, or "fake-list" for a paragraph/div/span whose text looks like a manually bulleted or numbered list.</param>
/// <param name="ItemCount">The number of list items or apparent list-like lines detected.</param>
/// <param name="SampleText">A short sample of the content, used for editorial context in the finding message.</param>
public sealed record ListStructureFragment(
    SourceLocation Location,
    string Kind,
    int ItemCount,
    string SampleText) : ContentFragment(Location);

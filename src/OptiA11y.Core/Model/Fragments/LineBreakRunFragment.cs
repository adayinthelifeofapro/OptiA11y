namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this run of line breaks was found.</param>
/// <param name="ConsecutiveBreakCount">The number of consecutive `&lt;br&gt;` elements with no other content between them.</param>
public sealed record LineBreakRunFragment(
    SourceLocation Location,
    int ConsecutiveBreakCount) : ContentFragment(Location);

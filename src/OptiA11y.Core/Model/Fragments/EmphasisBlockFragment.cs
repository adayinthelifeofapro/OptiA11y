namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// A short block-level element (e.g. a &lt;p&gt;) whose entire visible text is bold or set in a
/// noticeably larger font, outside of any real heading element - a common editorial pattern for
/// faking a heading in rich text. Whether it is actually BEING USED as a heading is a judgement
/// call, so the corresponding rule reports NeedsReview.
/// </summary>
/// <param name="Location">Where this block was found.</param>
/// <param name="SampleText">The block's text, truncated for display.</param>
/// <param name="Length">The full text length, used to distinguish short heading-like text from a bold paragraph of body copy.</param>
public sealed record EmphasisBlockFragment(
    SourceLocation Location,
    string SampleText,
    int Length) : ContentFragment(Location);

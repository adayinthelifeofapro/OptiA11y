namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this fieldset was found.</param>
/// <param name="HasLegend">True when the fieldset has a &lt;legend&gt; child element.</param>
public sealed record FieldsetFragment(
    SourceLocation Location,
    bool HasLegend) : ContentFragment(Location);

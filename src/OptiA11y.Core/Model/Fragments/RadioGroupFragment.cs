namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// A group of radio (or checkbox) inputs that share a <c>name</c> attribute - which is what makes
/// them a semantic group in HTML - found to have no common wrapping &lt;fieldset&gt; at all.
/// Only emitted in that "no fieldset" case: a fieldset that wraps the group but lacks a
/// &lt;legend&gt; is already covered by the plain <see cref="FieldsetFragment"/> check, so this
/// type never overlaps with it.
/// </summary>
/// <param name="Location">Where the first input in the group was found.</param>
/// <param name="GroupName">The shared <c>name</c> attribute value.</param>
/// <param name="OptionCount">How many inputs share this name.</param>
public sealed record RadioGroupFragment(
    SourceLocation Location,
    string GroupName,
    int OptionCount) : ContentFragment(Location);

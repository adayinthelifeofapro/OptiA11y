using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Model;

/// <summary>
/// A normalised, adapter-produced representation of a single content item's accessibility-relevant
/// surface. Rules operate exclusively against this document and never against a CMS API or DOM,
/// which is what makes the rules engine fully unit testable without a CMS present.
/// </summary>
public sealed class AuditDocument
{
    public AuditDocument(string contentReference, IReadOnlyList<ContentFragment> fragments)
    {
        ContentReference = contentReference;
        Fragments = fragments;
    }

    /// <summary>The identifier of the content item this document was built from.</summary>
    public string ContentReference { get; }

    /// <summary>Every fragment discovered on the content item, across all properties and nested blocks.</summary>
    public IReadOnlyList<ContentFragment> Fragments { get; }

    public IEnumerable<T> Get<T>() where T : ContentFragment => Fragments.OfType<T>();
}

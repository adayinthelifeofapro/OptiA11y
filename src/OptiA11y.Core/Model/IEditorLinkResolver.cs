namespace OptiA11y.Core.Model;

/// <summary>
/// Port implemented by each host (PaaS, SaaS) to turn a <see cref="SourceLocation"/> into a
/// URL an editor can follow to fix the underlying content. Core has no notion of edit views
/// or routing, so this is deliberately left as an interface for the host to satisfy — the
/// deep-linking payload lives in <see cref="SourceLocation"/>, the resolution logic does not.
/// </summary>
public interface IEditorLinkResolver
{
    /// <summary>
    /// Builds an absolute or relative editor URL for the given location, or null if this
    /// host cannot resolve a link (e.g. the content is no longer available).
    /// </summary>
    string? ResolveEditorLink(SourceLocation location);
}

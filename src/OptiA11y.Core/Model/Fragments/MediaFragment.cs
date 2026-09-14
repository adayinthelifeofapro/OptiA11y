namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this media reference was found.</param>
/// <param name="Src">The media source URL or asset reference.</param>
/// <param name="MediaKind">The kind of media, e.g. "video" or "audio".</param>
/// <param name="HasCaptionsOrTranscript">True when a captions track or transcript link is present alongside the media.</param>
public sealed record MediaFragment(
    SourceLocation Location,
    string Src,
    string MediaKind,
    bool HasCaptionsOrTranscript) : ContentFragment(Location);

namespace OptiA11y.Core.Model.Fragments;

/// <param name="Location">Where this media reference was found.</param>
/// <param name="Src">The media source URL or asset reference.</param>
/// <param name="MediaKind">The kind of media, e.g. "video" or "audio".</param>
/// <param name="HasCaptionsOrTranscript">True when a captions track or transcript link is present alongside the media.</param>
/// <param name="Autoplay">True when the autoplay attribute is present.</param>
/// <param name="Muted">True when the muted attribute is present.</param>
/// <param name="HasControls">True when the controls attribute is present, giving the user a way to pause/stop playback.</param>
/// <param name="HasDescriptionTrack">True when a track with kind="descriptions" is present, indicating audio description for video.</param>
/// <param name="HasSignLanguageTrack">True when a track referencing sign language (kind="sign" or a label/srclang mentioning "sign language") is present.</param>
/// <param name="HasExtendedDescriptionTrack">True when a descriptions track's label mentions "extended", distinguishing it from a standard-length audio-description track.</param>
/// <param name="TranscriptHref">The href of the nearest sibling link mentioning "transcript", if any.</param>
/// <param name="TranscriptLinkText">The visible text of the nearest sibling transcript link, if any.</param>
/// <param name="HasFlashIndicator">True when the source URL or surrounding class/data attributes mention flash/strobe/flicker, a heuristic signal that the media may contain rapid flashing.</param>
public sealed record MediaFragment(
    SourceLocation Location,
    string Src,
    string MediaKind,
    bool HasCaptionsOrTranscript,
    bool Autoplay = false,
    bool Muted = false,
    bool HasControls = true,
    bool HasDescriptionTrack = false,
    bool HasSignLanguageTrack = false,
    bool HasExtendedDescriptionTrack = false,
    string? TranscriptHref = null,
    string? TranscriptLinkText = null,
    bool HasFlashIndicator = false) : ContentFragment(Location);

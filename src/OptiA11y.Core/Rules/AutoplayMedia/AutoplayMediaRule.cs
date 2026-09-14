using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AutoplayMedia;

/// <summary>
/// Flags audio or video that autoplays with sound and gives the user no way to stop, pause, or
/// control its volume (no <c>controls</c> attribute) - a deterministic structural fact about the
/// markup, so this reports <see cref="Confidence.Fail"/>. WCAG 1.4.2 allows this only when the
/// user has a mechanism to stop it within three seconds; a missing controls attribute means no
/// such mechanism exists at all.
/// </summary>
public sealed class AutoplayMediaRule : IContentRule
{
    public string RuleId => "autoplay-media";

    public string SuccessCriterion => "1.4.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!media.Autoplay || media.Muted || media.HasControls)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.Fail,
                media.Location,
                $"This {media.MediaKind} autoplays with sound and has no controls attribute, giving the user no way to pause, stop, or mute it. Add controls, or remove autoplay.");
        }
    }
}

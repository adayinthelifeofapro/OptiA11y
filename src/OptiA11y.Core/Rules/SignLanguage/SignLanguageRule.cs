using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SignLanguage;

/// <summary>
/// Flags prerecorded video with no sign-language track reference (WCAG 1.2.6, AAA). Detecting a
/// track referencing sign language is deterministic, but whether this particular video needs a
/// sign-language interpretation is a judgement about the content this rule cannot make, so it
/// always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class SignLanguageRule : IContentRule
{
    public string RuleId => "sign-language";

    public string SuccessCriterion => "1.2.6";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var media in document.Get<MediaFragment>())
        {
            if (!string.Equals(media.MediaKind, "video", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (media.HasSignLanguageTrack)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                media.Location,
                "This video has no sign-language track reference. If a sign-language interpretation is warranted for this content, provide one (WCAG 1.2.6, AAA).");
        }
    }
}

using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ImageOfText;

/// <summary>
/// Flags images whose alt text reads like a full sentence or heading, which is a common signal
/// that the image itself contains rendered text (a screenshot of a quote, a graphic headline)
/// rather than a photo or illustration being described. This is always a judgement call — the
/// alt text could simply be a thorough description — so it is <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class ImageOfTextRule : IContentRule
{
    private const int SentenceLikeMinLength = 60;

    private static readonly Regex SentencePunctuation = new(@"[.!?]\s*$", RegexOptions.Compiled);

    public string RuleId => "image-of-text";

    public string SuccessCriterion => "1.4.5";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var image in document.Get<ImageFragment>())
        {
            if (string.IsNullOrWhiteSpace(image.AltText))
            {
                continue;
            }

            var alt = image.AltText.Trim();
            var wordCount = alt.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;

            var looksLikeSentence = alt.Length >= SentenceLikeMinLength
                && wordCount >= 8
                && SentencePunctuation.IsMatch(alt);

            if (looksLikeSentence)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    image.Location,
                    "This image's alt text reads like a full sentence or heading. If the image itself contains rendered text (a quote graphic, a text screenshot), consider using real text instead of an image, or confirm the alt text fully reproduces it.");
            }
        }
    }
}

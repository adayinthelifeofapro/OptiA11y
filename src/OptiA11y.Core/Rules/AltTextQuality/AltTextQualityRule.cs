using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AltTextQuality;

/// <summary>
/// Evaluates alt text quality on images. Only the entirely-missing-attribute case is
/// deterministic enough to be a <see cref="Confidence.Fail"/>; every other judgement here is a
/// heuristic about editorial intent and must surface as <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class AltTextQualityRule : IContentRule
{
    private const int RedundantPhraseMaxLength = 150;

    private static readonly Regex FilenamePattern = new(
        @"^[\w\-]+\.(png|jpe?g|gif|svg|webp|bmp|tiff?)$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly string[] RedundantPhrases =
    {
        "image of",
        "picture of",
        "photo of",
        "graphic of",
        "icon of",
        "image:",
        "photo:"
    };

    public string RuleId => "alt-text-quality";

    public string SuccessCriterion => "1.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var image in document.Get<ImageFragment>())
        {
            var finding = EvaluateImage(image);
            if (finding is not null)
            {
                yield return finding;
            }
        }
    }

    private Finding? EvaluateImage(ImageFragment image)
    {
        // Attribute entirely absent is a deterministic, unambiguous violation.
        if (image.AltText is null)
        {
            return Fail(image, "This image has no alt attribute at all. Add one, or mark it decorative if it conveys no information.");
        }

        // An empty alt is a valid, deliberate way to mark an image as decorative — but only
        // when the surrounding context supports that. We cannot be certain, so this is a review.
        if (image.AltText.Length == 0)
        {
            return image.IsDecorativeCandidate
                ? null
                : NeedsReview(image, "This image has empty alt text but doesn't look decorative. Confirm it truly conveys no information, or add a description.");
        }

        var altText = image.AltText.Trim();

        if (FilenamePattern.IsMatch(altText))
        {
            return NeedsReview(image, $"The alt text \"{image.AltText}\" looks like a filename rather than a description. Consider describing what the image shows.");
        }

        var lowerAlt = altText.ToLowerInvariant();
        foreach (var phrase in RedundantPhrases)
        {
            if (lowerAlt.StartsWith(phrase, StringComparison.Ordinal))
            {
                return NeedsReview(image, $"The alt text starts with the redundant phrase \"{phrase}\". Screen readers already announce images as images; describe the content instead.");
            }
        }

        if (altText.Length > RedundantPhraseMaxLength)
        {
            return NeedsReview(image, $"The alt text is {altText.Length} characters long, which may be excessive. Consider a shorter description, moving detail to surrounding text if needed.");
        }

        return null;
    }

    private Finding Fail(ImageFragment image, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Critical,
        Confidence.Fail,
        image.Location,
        message);

    private Finding NeedsReview(ImageFragment image, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.NeedsReview,
        image.Location,
        message);
}

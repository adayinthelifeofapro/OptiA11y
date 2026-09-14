using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TitleAttributeMisuse;

/// <summary>
/// Flags a link or button whose <c>title</c> attribute either duplicates its own visible text
/// (redundant - screen readers may announce it twice) or is the ONLY accessible name the element
/// has (unreliable - many screen readers, and all touch devices, don't expose title text at
/// all). Both checks are about how well the naming mechanism actually works for users, which is
/// a judgement call, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class TitleAttributeMisuseRule : IContentRule
{
    public string RuleId => "title-attribute-misuse";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (string.IsNullOrWhiteSpace(link.TitleAttribute))
            {
                continue;
            }

            var visibleText = link.Text.Trim();
            if (visibleText.Length == 0)
            {
                yield return NeedsReview(link.Location, "This link has no visible text and relies on its title attribute as the only accessible name. Many screen readers and all touch devices don't expose title text - add visible text or an aria-label instead.");
            }
            else if (string.Equals(visibleText, link.TitleAttribute!.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                yield return NeedsReview(link.Location, $"This link's title attribute (\"{link.TitleAttribute}\") just repeats its visible text. Some screen readers announce both, reading the label twice. Remove the redundant title, or use it to add extra information.");
            }
        }

        foreach (var button in document.Get<ButtonFragment>())
        {
            if (string.IsNullOrWhiteSpace(button.TitleAttribute))
            {
                continue;
            }

            if (!button.HasAccessibleName)
            {
                yield return NeedsReview(button.Location, "This control has no accessible name other than its title attribute. Many screen readers and all touch devices don't expose title text - add visible text or an aria-label instead.");
            }
        }
    }

    private Finding NeedsReview(SourceLocation location, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.NeedsReview,
        location,
        message);
}

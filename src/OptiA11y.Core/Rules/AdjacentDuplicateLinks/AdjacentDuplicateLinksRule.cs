using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AdjacentDuplicateLinks;

/// <summary>
/// Flags two adjacent links that point at the same destination, where one has no visible text
/// of its own (typically an image-only link) immediately followed or preceded by a text link to
/// the same target. Screen reader users hear two separate stops for what is effectively one
/// piece of navigation. Adjacency is inferred from consecutive ordinals within the same property,
/// which is a heuristic proxy for DOM adjacency, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class AdjacentDuplicateLinksRule : IContentRule
{
    public string RuleId => "adjacent-duplicate-links";

    public string SuccessCriterion => "2.4.4";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var links = document.Get<LinkFragment>()
            .OrderBy(l => l.Location.Ordinal)
            .ToList();

        for (var i = 0; i < links.Count - 1; i++)
        {
            var first = links[i];
            var second = links[i + 1];

            if (!string.Equals(first.Href, second.Href, StringComparison.OrdinalIgnoreCase)
                || first.Href.Length == 0)
            {
                continue;
            }

            var firstIsEmpty = first.Text.Trim().Length == 0;
            var secondIsEmpty = second.Text.Trim().Length == 0;

            // Only flag when exactly one of the pair has no visible text - that's the "redundant
            // announcement" shape (e.g. an image link followed by a text link to the same page).
            // Two links with the same visible text to the same target are not a problem at all.
            if (firstIsEmpty == secondIsEmpty)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                second.Location,
                "This link and the one immediately before it point at the same destination, with only one of the pair carrying visible text. Consider combining them into a single link so assistive technology users don't hear the same destination announced twice.");
        }
    }
}

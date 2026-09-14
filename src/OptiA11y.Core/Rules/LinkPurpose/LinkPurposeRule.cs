using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LinkPurpose;

/// <summary>
/// Evaluates link text for descriptiveness out of context. Both checks here are heuristic
/// judgements about whether link text conveys purpose, so all findings are
/// <see cref="Confidence.NeedsReview"/>. Whether a link has an accessible name AT ALL (via
/// visible text, aria-label, or an inner image's alt) is a separate, deterministic concern
/// handled by <see cref="OptiA11y.Core.Rules.LinkName.LinkNameRule"/> - a link with no visible
/// text but a valid aria-label has nothing wrong with it from THIS rule's point of view.
/// </summary>
public sealed class LinkPurposeRule : IContentRule
{
    private static readonly HashSet<string> NonDescriptivePhrases = new(StringComparer.OrdinalIgnoreCase)
    {
        "click here",
        "here",
        "read more",
        "learn more",
        "more",
        "more info",
        "more information",
        "link",
        "this link",
        "this page",
        "go",
        "continue"
    };

    private static readonly Regex BareUrlPattern = new(
        @"^(https?://|www\.)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string RuleId => "link-purpose";

    public string SuccessCriterion => "2.4.4";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var links = document.Get<LinkFragment>().ToList();

        foreach (var link in links)
        {
            var text = link.Text.Trim();

            if (NonDescriptivePhrases.Contains(text))
            {
                yield return NeedsReview(link, $"The link text \"{link.Text}\" doesn't describe its destination out of context. Consider naming what the user will find.");
                continue;
            }

            if (BareUrlPattern.IsMatch(text))
            {
                yield return NeedsReview(link, "This link's visible text is a raw URL. Consider a descriptive label instead.");
            }
        }

        foreach (var group in links.GroupBy(l => l.Text.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            if (group.Key.Length == 0)
            {
                continue;
            }

            var distinctTargets = group.Select(l => l.Href).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (distinctTargets.Count > 1)
            {
                foreach (var link in group)
                {
                    yield return NeedsReview(
                        link,
                        $"The link text \"{group.Key}\" is used for {distinctTargets.Count} different destinations on this content. Identical link text should lead to the same place.");
                }
            }
        }
    }

    private Finding NeedsReview(LinkFragment link, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Minor,
        Confidence.NeedsReview,
        link.Location,
        message);
}

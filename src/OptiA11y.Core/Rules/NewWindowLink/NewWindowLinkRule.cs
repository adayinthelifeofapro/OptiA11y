using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.NewWindowLink;

/// <summary>
/// Flags a link with <c>target="_blank"</c> (or any non-self/parent/top target) whose accessible
/// name gives no warning that it opens in a new window/tab. Whether the accessible name actually
/// contains an adequate warning phrase is a judgement call about natural language, so this always
/// reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class NewWindowLinkRule : IContentRule
{
    private static readonly string[] WarningPhrases =
    {
        "new window", "new tab", "opens in a new", "(opens", "external site", "external link"
    };

    public string RuleId => "new-window-link";

    public string SuccessCriterion => "3.2.5";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (string.IsNullOrWhiteSpace(link.Target)
                || string.Equals(link.Target, "_self", StringComparison.OrdinalIgnoreCase)
                || string.Equals(link.Target, "_parent", StringComparison.OrdinalIgnoreCase)
                || string.Equals(link.Target, "_top", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var text = link.Text.Trim();
            var hasWarning = WarningPhrases.Any(phrase => text.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                || (link.TitleAttribute is not null && WarningPhrases.Any(phrase => link.TitleAttribute.Contains(phrase, StringComparison.OrdinalIgnoreCase)));

            if (hasWarning)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                link.Location,
                $"This link opens in a new window/tab (target=\"{link.Target}\") but its text/title gives no warning of that. Users following it unexpectedly lose their place on this page.");
        }
    }
}

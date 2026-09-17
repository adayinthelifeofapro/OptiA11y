using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AbbreviationExpansion;

/// <summary>
/// Flags an acronym-shaped token (2-6 upper-case letters, e.g. "API", "WCAG") that appears
/// repeatedly across a property's text with no nearby expansion (a parenthetical spelling-out
/// the first time it's used, or an `&lt;abbr title&gt;`). Screen reader users who don't
/// recognise the acronym have no way to discover what it stands for. Deciding whether a given
/// acronym is well-known enough not to need expansion is an editorial judgement call, so this
/// always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class AbbreviationExpansionRule : IContentRule
{
    private const int MinimumRepeatCount = 2;

    private static readonly Regex AcronymToken = new(@"\b[A-Z]{2,6}\b", RegexOptions.Compiled);

    public string RuleId => "abbreviation-expansion";

    public string SuccessCriterion => "3.1.4";

    public WcagLevel Level => WcagLevel.AAA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        var texts = document.Get<TextFragment>().ToList();
        var occurrences = new Dictionary<string, List<TextFragment>>(StringComparer.Ordinal);

        foreach (var text in texts)
        {
            foreach (Match match in AcronymToken.Matches(text.Text))
            {
                var token = match.Value;

                if (!occurrences.TryGetValue(token, out var list))
                {
                    list = new List<TextFragment>();
                    occurrences[token] = list;
                }

                list.Add(text);
            }
        }

        foreach (var (token, fragments) in occurrences)
        {
            if (fragments.Count < MinimumRepeatCount)
            {
                continue;
            }

            var hasExpansion = fragments.Any(f => f.Text.Contains($"({token})", StringComparison.Ordinal) ||
                                                    f.Text.Contains($"{token} (", StringComparison.Ordinal));
            if (hasExpansion)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragments[0].Location,
                $"The acronym \"{token}\" appears {fragments.Count} times in this content with no expansion (e.g. a parenthetical spelling it out, or an <abbr title> element). Readers unfamiliar with it have no way to discover what it stands for.");
        }
    }
}

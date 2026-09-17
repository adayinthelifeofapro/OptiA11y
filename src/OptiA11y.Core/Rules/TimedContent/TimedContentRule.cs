using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.TimedContent;

/// <summary>
/// Flags text describing a session or countdown time limit (e.g. "your session will expire in
/// 10 minutes", "this offer ends in 00:59"), which WCAG 2.2.1 requires the user be able to turn
/// off, adjust, or extend. Recognising the phrasing is a heuristic over free text, and whether an
/// adjust/extend mechanism actually exists elsewhere on the page is outside what this rule can
/// see, so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class TimedContentRule : IContentRule
{
    private static readonly Regex TimeLimitPattern = new(
        @"\b(session|page|form)\b[^.]{0,40}\b(expir\w*|time\s*out|times\s*out)\b" +
        @"|\b(expir\w*|time\s*out|times\s*out)\b[^.]{0,40}\b(minute|second|hour)s?\b" +
        @"|\bcountdown\b" +
        @"|\bthis (offer|deal|sale)\b[^.]{0,40}\bends?\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string RuleId => "timed-content";

    public string SuccessCriterion => "2.2.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            if (!TimeLimitPattern.IsMatch(text.Text))
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                text.Location,
                "This text describes a session or countdown time limit. Confirm the user can turn it off, adjust it, or extend it before it expires (WCAG 2.2.1).");
        }
    }
}

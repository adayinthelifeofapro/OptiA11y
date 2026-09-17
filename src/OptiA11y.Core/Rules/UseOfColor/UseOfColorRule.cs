using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.UseOfColor;

/// <summary>
/// Flags text that refers to color as the only way to identify something (e.g. "click the red
/// button", "fields shown in red are required") - WCAG 1.4.1 requires an additional non-color
/// cue such as text, an icon, or a pattern. Recognising the phrasing is a heuristic over free
/// text - the rule cannot see whether a non-color cue also exists elsewhere on the page - so
/// this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class UseOfColorRule : IContentRule
{
    private static readonly Regex ColorReferencePattern = new(
        @"\b(red|green|blue|yellow|orange|purple|pink)\b[^.]{0,30}\b(button|link|field|text|item|option|icon|indicator)s?\b" +
        @"|\b(button|link|field|text|item|option|icon|indicator)s?\b[^.]{0,30}\b(shown|marked|highlighted|colou?red)\b[^.]{0,20}\b(red|green|blue|yellow|orange|purple|pink)\b",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string RuleId => "use-of-color";

    public string SuccessCriterion => "1.4.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            if (!ColorReferencePattern.IsMatch(text.Text))
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
                "This text identifies something by color alone. Confirm there's also a non-color cue (text, icon, or pattern), or add one (WCAG 1.4.1).");
        }
    }
}

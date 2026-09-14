using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SensoryCharacteristics;

/// <summary>
/// Flags instructions that rely on shape, position, or color to identify content (e.g. "click the
/// green button on the right", "see the box below"), which a screen reader user or someone with
/// low vision may not be able to act on. Whether a phrase like this actually leaves the user
/// stranded depends on whether another cue is also given nearby, which this rule cannot see, so
/// it always reports NeedsReview.
/// </summary>
public sealed class SensoryCharacteristicsRule : IContentRule
{
    private static readonly Regex[] SensoryPhrasePatterns =
    {
        new(@"\b(button|link|icon|tab|menu|box|panel|image)\s+(on|to)\s+the\s+(left|right|top|bottom)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"\b(click|tap|select|press)\s+the\s+(round|square|circular|red|green|blue|yellow|orange|purple)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"\bthe\s+(box|button|link|icon)\s+(above|below)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        new(@"\b(scroll|swipe)\s+(up|down|left|right)\s+to\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)
    };

    public string RuleId => "sensory-characteristics";

    public string SuccessCriterion => "1.3.3";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            foreach (var pattern in SensoryPhrasePatterns)
            {
                var match = pattern.Match(text.Text);
                if (match.Success)
                {
                    yield return new Finding(
                        RuleId,
                        SuccessCriterion,
                        Level,
                        Severity.Minor,
                        Confidence.NeedsReview,
                        text.Location,
                        $"The phrase \"{match.Value}\" identifies content by its shape, position, or color alone. Confirm there's also a text label or name a screen reader user could follow instead.");
                    break;
                }
            }
        }
    }
}

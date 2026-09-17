using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.SelectOptionQuality;

/// <summary>
/// Flags a &lt;select&gt; whose first option reads like a "please choose" placeholder acting as
/// a label rather than a real, selectable value, or which has one or more options with empty
/// text. Whether the first option is genuinely acting as a placeholder (as opposed to a
/// legitimate default choice) is a judgement call about editorial intent, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class SelectOptionQualityRule : IContentRule
{
    private static readonly string[] PlaceholderOptionPhrases =
    {
        "select", "choose", "please select", "please choose", "-- select --", "pick one", "select an option"
    };

    public string RuleId => "select-option-quality";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (field.ControlType != "select")
            {
                continue;
            }

            if (field.FirstOptionText is not null
                && PlaceholderOptionPhrases.Any(phrase => field.FirstOptionText.Contains(phrase, StringComparison.OrdinalIgnoreCase)))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    field.Location,
                    $"This <select>'s first option (\"{field.FirstOptionText}\") reads like a placeholder prompt rather than a real choice. If it's meant to act as the field's label, use a real <label> instead - relying on option text alone is unreliable across assistive technologies.");
            }

            if (field.HasEmptyOptionText)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    field.Location,
                    "This <select> has one or more <option> elements with no visible text. Screen reader users will hear nothing when that option is focused.");
            }
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.RedundantEntry;

/// <summary>
/// Flags a field whose label/id/name/placeholder suggests it's asking the user to re-enter
/// information already provided earlier in the same step (e.g. "confirm email", "verify
/// password") without an autocomplete token that would let the browser fill it automatically.
/// WCAG 2.2's 3.3.7 asks authors to avoid this kind of redundant entry where possible. Whether a
/// "confirm" field is truly avoidable redundant entry (as opposed to a legitimate double-entry
/// security check) is an editorial judgement call, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class RedundantEntryRule : IContentRule
{
    private static readonly string[] ConfirmationKeywords =
    {
        "confirm", "verify", "re-enter", "reenter", "repeat"
    };

    public string RuleId => "redundant-entry";

    public string SuccessCriterion => "3.3.7";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (!string.IsNullOrWhiteSpace(field.AutocompleteToken))
            {
                continue;
            }

            var haystack = $"{field.LabelText} {field.PlaceholderText}";
            if (!ConfirmationKeywords.Any(keyword => haystack.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                field.Location,
                $"This {field.ControlType} looks like it asks the user to re-enter information already provided elsewhere in the form (e.g. \"confirm\"/\"verify\"). WCAG 3.3.7 recommends avoiding this redundant entry where possible, or at least supporting autocomplete/autofill so the browser can fill it automatically.");
        }
    }
}

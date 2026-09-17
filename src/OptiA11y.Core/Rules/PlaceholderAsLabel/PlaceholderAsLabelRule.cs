using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PlaceholderAsLabel;

/// <summary>
/// Flags a form field whose only accessible name comes from a <c>placeholder</c> attribute -
/// the placeholder text vanishes as soon as the user starts typing, and many assistive
/// technologies don't announce it consistently as a name at all. <c>HasAccessibleName</c> never
/// treats a placeholder as an accessible name (only a real &lt;label&gt;, <c>aria-label</c>, or
/// <c>aria-labelledby</c> count), so a field relying solely on its placeholder always has
/// <c>HasAccessibleName == false</c> and would otherwise only surface via
/// <see cref="OptiA11y.Core.Rules.FormLabels.FormLabelRule"/>'s generic "no accessible name"
/// message. This rule instead specifically calls out the placeholder-as-label anti-pattern with a
/// more actionable message. Both facts are structural, so this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class PlaceholderAsLabelRule : IContentRule
{
    public string RuleId => "placeholder-as-label";

    public string SuccessCriterion => "3.3.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (string.IsNullOrWhiteSpace(field.PlaceholderText))
            {
                continue;
            }

            // Only relevant when there is no real label at all - a placeholder alongside a real
            // label is a fine, if redundant, hint rather than a naming problem.
            if (field.LabelText is not null)
            {
                continue;
            }

            if (field.HasAccessibleName)
            {
                // Has an aria-label/aria-labelledby/ancestor label instead of a <label> -
                // FormLabelRule and this rule are mutually exclusive on that dimension.
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                field.Location,
                $"This {field.ControlType} relies on its placeholder text (\"{field.PlaceholderText}\") as its only accessible name. Placeholder text disappears once the user starts typing and isn't a substitute for a real <label>.");
        }
    }
}

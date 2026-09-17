using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ReadonlyDisabledMisuse;

/// <summary>
/// Flags a field marked <c>disabled</c> or <c>aria-disabled="true"</c> that also looks required
/// (visual asterisk, or a <c>required</c>/<c>aria-required="true"</c> attribute). A disabled
/// required field is usually a sign that conditional-display logic hid the field incorrectly, or
/// that the required marker is stale copy - either way, it's a combination worth a human look
/// rather than a certain defect, so this always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class ReadonlyDisabledMisuseRule : IContentRule
{
    public string RuleId => "readonly-disabled-misuse";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (!(field.IsDisabled || field.AriaDisabled))
            {
                continue;
            }

            if (!(field.IsRequired || field.HasVisualRequiredIndicator))
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
                $"This {field.ControlType} is disabled but also appears to be marked required. A disabled required field usually indicates the field was hidden by conditional logic without clearing its required state, which can block form submission or confuse assistive technology users.");
        }
    }
}

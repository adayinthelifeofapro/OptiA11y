using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ErrorIdentification;

/// <summary>
/// Flags a form field marked with <c>aria-invalid="true"</c> (indicating validation error styling
/// or state has been applied to it) that has no <c>aria-describedby</c> pointing at an error
/// message. Whether the field genuinely has associated error text elsewhere on the rendered page
/// is something this rule cannot see - it only knows the field's own markup - so this always
/// reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class ErrorIdentificationRule : IContentRule
{
    public string RuleId => "error-identification";

    public string SuccessCriterion => "3.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (!field.AriaInvalid || field.HasAriaDescribedBy)
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
                $"This {field.ControlType} is marked aria-invalid=\"true\" but has no aria-describedby pointing at an error message. Without that association, screen reader users may not hear why the field is invalid.");
        }
    }
}

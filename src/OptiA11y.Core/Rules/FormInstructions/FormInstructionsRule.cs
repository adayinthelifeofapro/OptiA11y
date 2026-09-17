using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FormInstructions;

/// <summary>
/// Flags a field with a specific expected format (a <c>pattern</c> attribute, or an input type
/// like email/tel/date that implies a format) that has no <c>aria-describedby</c> pointing at
/// instruction text explaining that format. Whether format guidance actually exists elsewhere on
/// the rendered page, and whether it would be genuinely necessary for this particular field, is a
/// judgement call this rule cannot make with certainty, so it always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FormInstructionsRule : IContentRule
{
    public string RuleId => "form-instructions";

    public string SuccessCriterion => "3.3.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (!field.HasPatternOrFormatConstraint || field.HasAriaDescribedBy)
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
                $"This {field.ControlType} expects a specific format but has no aria-describedby pointing at instruction text explaining it. Consider adding format guidance and associating it with the field.");
        }
    }
}

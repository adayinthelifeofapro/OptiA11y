using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FormLabels;

/// <summary>
/// Evaluates form controls (inputs, selects, textareas) for an accessible name. Whether a
/// control has an associated label, aria-label, or aria-labelledby is a deterministic structural
/// fact, so this rule reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class FormLabelRule : IContentRule
{
    private static readonly HashSet<string> ExemptInputTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "hidden"
    };

    public string RuleId => "form-labels";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (field.InputType is not null && ExemptInputTypes.Contains(field.InputType))
            {
                continue;
            }

            if (!field.HasAccessibleName)
            {
                var descriptor = field.InputType is null ? field.ControlType : $"{field.ControlType} (type={field.InputType})";
                yield return Fail(field, $"This {descriptor} has no associated label, aria-label, or aria-labelledby. Screen reader users won't know its purpose.");
            }
        }
    }

    private Finding Fail(FormFieldFragment field, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Critical,
        Confidence.Fail,
        field.Location,
        message);
}

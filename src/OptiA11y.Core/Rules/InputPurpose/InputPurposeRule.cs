using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.InputPurpose;

/// <summary>
/// Flags a form field whose id/name/placeholder/label suggests it collects a common piece of
/// identity information (email, phone, name, address, etc.) but has no <c>autocomplete</c>
/// attribute, which WCAG 1.3.5 asks for so browsers and assistive technology can help fill it in.
/// The purpose is inferred from naming conventions, which vary widely, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class InputPurposeRule : IContentRule
{
    public string RuleId => "input-purpose";

    public string SuccessCriterion => "1.3.5";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (field.InferredPurposeCategory is null)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(field.AutocompleteToken))
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
                $"This field looks like it collects a \"{field.InferredPurposeCategory}\" value but has no autocomplete attribute. Adding autocomplete=\"{field.InferredPurposeCategory}\" lets browsers and assistive technology help fill it in.");
        }
    }
}

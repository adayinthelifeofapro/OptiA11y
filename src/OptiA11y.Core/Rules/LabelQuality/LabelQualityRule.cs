using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LabelQuality;

/// <summary>
/// Flags form field labels that are generic placeholders ("Field 1", "Text", "Input") rather
/// than descriptive of the field's purpose. Whether a short label is genuinely descriptive
/// depends on context the tool can't see, so this is <see cref="Confidence.NeedsReview"/> only.
/// </summary>
public sealed class LabelQualityRule : IContentRule
{
    private static readonly Regex GenericLabelPattern = new(
        @"^(field|input|text( ?box)?|label|value)\s*\d*$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public string RuleId => "label-quality";

    public string SuccessCriterion => "2.4.6";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (string.IsNullOrWhiteSpace(field.LabelText))
            {
                continue;
            }

            var label = field.LabelText.Trim();
            if (GenericLabelPattern.IsMatch(label))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    field.Location,
                    $"This field's label, \"{field.LabelText}\", is generic and doesn't describe what the field is for. Consider a label that names the expected value, such as \"Email address\" instead of \"Field 1\".");
            }
        }
    }
}

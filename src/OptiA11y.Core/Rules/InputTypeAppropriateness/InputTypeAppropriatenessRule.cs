using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.InputTypeAppropriateness;

/// <summary>
/// Flags an &lt;input&gt; whose id/name/placeholder/label suggests a well-known data type
/// (email, phone) but whose <c>type</c> attribute is a generic "text" rather than the matching
/// semantic type ("email", "tel"). The semantic type triggers better virtual keyboards, native
/// validation, and autofill for assistive technology and mobile users. Because the purpose is
/// inferred from naming conventions rather than observed directly, this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class InputTypeAppropriatenessRule : IContentRule
{
    private static readonly Dictionary<string, string> CategoryToInputType = new(StringComparer.OrdinalIgnoreCase)
    {
        ["email"] = "email",
        ["tel"] = "tel"
    };

    public string RuleId => "input-type-appropriateness";

    public string SuccessCriterion => "1.3.5";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (field.ControlType != "input" || field.InferredPurposeCategory is null)
            {
                continue;
            }

            if (!CategoryToInputType.TryGetValue(field.InferredPurposeCategory, out var expectedType))
            {
                continue;
            }

            if (string.Equals(field.InputType, expectedType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.Equals(field.InputType, "text", StringComparison.OrdinalIgnoreCase))
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
                $"This input looks like it collects a \"{field.InferredPurposeCategory}\" value but uses type=\"text\" instead of type=\"{expectedType}\". The semantic type gives users a better virtual keyboard and native validation.");
        }
    }
}

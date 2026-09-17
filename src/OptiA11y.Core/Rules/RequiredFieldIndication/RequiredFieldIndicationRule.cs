using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.RequiredFieldIndication;

/// <summary>
/// Flags a form field that visually signals it's required (an asterisk in its label or
/// placeholder text) but has no <c>required</c> attribute or <c>aria-required="true"</c>.
/// Screen reader users won't hear that the field is mandatory until they submit and encounter an
/// error. Whether the visual marker actually communicates "required" (rather than, say, a
/// footnote reference) is a judgement call about intent, so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class RequiredFieldIndicationRule : IContentRule
{
    public string RuleId => "required-field-indication";

    public string SuccessCriterion => "3.3.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var field in document.Get<FormFieldFragment>())
        {
            if (!field.HasVisualRequiredIndicator || field.IsRequired)
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
                $"This {field.ControlType}'s label or placeholder looks like it marks the field as required (e.g. an asterisk), but it has no required attribute or aria-required=\"true\". Screen reader users won't know it's mandatory until they submit the form.");
        }
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.FieldsetLegend;

/// <summary>
/// Flags a &lt;fieldset&gt; with no &lt;legend&gt;, and a group of radio/checkbox inputs sharing
/// a <c>name</c> (which is what makes them a semantic group in HTML) with no wrapping fieldset
/// at all. Both are structural facts about the markup, so this reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class FieldsetLegendRule : IContentRule
{
    public string RuleId => "fieldset-legend";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fieldset in document.Get<FieldsetFragment>())
        {
            if (fieldset.HasLegend)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                fieldset.Location,
                "This <fieldset> has no <legend>. Screen reader users won't hear a name for the group of controls it contains.");
        }

        foreach (var radioGroup in document.Get<RadioGroupFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                radioGroup.Location,
                $"These {radioGroup.OptionCount} radio/checkbox inputs share the name \"{radioGroup.GroupName}\", making them a group, but aren't wrapped in a <fieldset> with a <legend>. Screen reader users won't hear what the group of options is for.");
        }
    }
}

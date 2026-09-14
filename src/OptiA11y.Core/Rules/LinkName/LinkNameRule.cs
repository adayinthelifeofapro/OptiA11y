using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LinkName;

/// <summary>
/// Flags a link with no accessible name at all: no visible text, no aria-label/aria-labelledby,
/// and no image with alt text inside it. Unlike <see cref="OptiA11y.Core.Rules.LinkPurpose.LinkPurposeRule"/>
/// (which judges whether the accessible name is a GOOD one), whether an accessible name exists
/// at all is a deterministic structural fact, so this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class LinkNameRule : IContentRule
{
    public string RuleId => "link-name";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (link.HasAccessibleName)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Critical,
                Confidence.Fail,
                link.Location,
                "This link has no accessible name: no visible text, no aria-label, and no image with alt text inside it. Screen reader users will have nothing to announce for it.");
        }
    }
}

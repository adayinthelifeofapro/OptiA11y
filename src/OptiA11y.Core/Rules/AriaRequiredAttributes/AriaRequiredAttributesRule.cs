using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.AriaRequiredAttributes;

/// <summary>
/// Flags an element whose ARIA role requires specific state/property attributes (e.g.
/// <c>role="checkbox"</c> requires <c>aria-checked</c>) that are absent. The ARIA specification's
/// required-attributes table is a fixed vocabulary (scoped here to a small, well-established
/// subset), so membership is a structural fact and this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class AriaRequiredAttributesRule : IContentRule
{
    public string RuleId => "aria-required-attributes";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            foreach (var missingAttribute in fragment.MissingRequiredAttributes)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Major,
                    Confidence.Fail,
                    fragment.Location,
                    $"This <{fragment.TagName}> has role=\"{fragment.Role}\", which requires the \"{missingAttribute}\" attribute, but it is missing. Assistive technology cannot announce this widget's state without it.");
            }
        }
    }
}

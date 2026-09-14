using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LanguageAttribute;

/// <summary>
/// Evaluates declared <c>lang</c> attributes for well-formedness. Whether a lang attribute
/// value is a plausible BCP 47 tag is a deterministic structural check, so this rule reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class LanguageAttributeRule : IContentRule
{
    public string RuleId => "language-attribute";

    public string SuccessCriterion => "3.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<LanguageAttributeFragment>())
        {
            if (string.IsNullOrWhiteSpace(fragment.LanguageCode))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Critical,
                    Confidence.Fail,
                    fragment.Location,
                    "The page's lang attribute is empty. Screen readers cannot determine the correct pronunciation and language rules without it.");
            }
            else if (!fragment.IsWellFormed)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Major,
                    Confidence.Fail,
                    fragment.Location,
                    $"The lang attribute value \"{fragment.LanguageCode}\" doesn't look like a valid language tag (e.g. \"en\", \"en-GB\"). Assistive technology may mispronounce content as a result.");
            }
        }
    }
}

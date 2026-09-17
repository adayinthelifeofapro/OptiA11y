using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DuplicateId;

/// <summary>
/// Flags an <c>id</c> value that appears on more than one element within the same property.
/// Duplicate ids are a deterministic structural fact — assistive technology and
/// <c>aria-labelledby</c>/<c>aria-describedby</c>/<c>for</c> references can only resolve to the
/// first match, silently breaking any reference to the second — so this rule reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class DuplicateIdRule : IContentRule
{
    public string RuleId => "duplicate-id";

    public string SuccessCriterion => "4.1.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var duplicate in document.Get<DuplicateIdFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                duplicate.Location,
                $"The id \"{duplicate.Id}\" is used on {duplicate.OccurrenceCount} elements in this property. Ids must be unique, or assistive technology and label/ARIA references can resolve to the wrong element.");
        }
    }
}

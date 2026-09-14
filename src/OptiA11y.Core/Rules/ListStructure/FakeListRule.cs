using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ListStructure;

/// <summary>
/// Flags paragraphs/divs/spans whose text is manually bulleted or numbered instead of using
/// semantic &lt;ul&gt;/&lt;ol&gt; markup. Screen readers can't announce list semantics (item count,
/// position) for these, so editors should be prompted to review and convert them. Whether such
/// text truly represents a list is an editorial judgement, so this rule reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class FakeListRule : IContentRule
{
    public string RuleId => "list-structure";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<ListStructureFragment>())
        {
            if (fragment.Kind != "fake-list")
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                fragment.Location,
                $"This text appears to be a manually formatted list ({fragment.ItemCount} items detected: \"{fragment.SampleText}\"). Consider using a real bulleted or numbered list so screen readers can announce its structure.");
        }
    }
}

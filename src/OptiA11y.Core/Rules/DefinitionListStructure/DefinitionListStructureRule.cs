using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DefinitionListStructure;

/// <summary>
/// Flags structural defects in &lt;dl&gt; definition lists: a &lt;dd&gt; appearing before any
/// &lt;dt&gt;, or a &lt;dt&gt; with no following &lt;dd&gt;. Both are deterministic structural facts,
/// so this rule reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class DefinitionListStructureRule : IContentRule
{
    public string RuleId => "definition-list-structure";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var list in document.Get<DefinitionListFragment>())
        {
            if (list.DescriptionBeforeTerm)
            {
                yield return Fail(list, "This definition list has a <dd> appearing before any <dt>. Every description should follow the term it describes.");
            }

            if (list.HasOrphanedTerm)
            {
                yield return Fail(list, "This definition list has a <dt> with no following <dd>. Every term should be followed by at least one description.");
            }
        }
    }

    private Finding Fail(DefinitionListFragment list, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        list.Location,
        message);
}

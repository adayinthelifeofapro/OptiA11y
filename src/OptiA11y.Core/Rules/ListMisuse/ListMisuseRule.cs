using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.ListMisuse;

/// <summary>
/// Flags structural defects in semantic lists: an empty &lt;ul&gt;/&lt;ol&gt; with no items, or a
/// list containing a direct element child that is not an &lt;li&gt;. Both are deterministic
/// structural facts — no editorial judgement is involved — so this rule reports
/// <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class ListMisuseRule : IContentRule
{
    public string RuleId => "list-misuse";

    public string SuccessCriterion => "1.3.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var list in document.Get<ListStructureFragment>())
        {
            if (list.Kind != "list")
            {
                continue;
            }

            if (list.ItemCount == 0)
            {
                yield return Fail(list, "This list has no <li> items. An empty list conveys nothing to a screen reader user and should either contain items or be removed.");
            }

            if (list.HasNonListItemChild)
            {
                yield return Fail(list, "This list has a direct child element that is not an <li>. Only <li> elements should appear directly inside a <ul>/<ol>; move other content inside a list item or outside the list.");
            }
        }
    }

    private Finding Fail(ListStructureFragment list, string message) => new(
        RuleId,
        SuccessCriterion,
        Level,
        Severity.Major,
        Confidence.Fail,
        list.Location,
        message);
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.RedundantRole;

/// <summary>
/// Flags an explicit <c>role</c> attribute that duplicates the element's native implicit role
/// (e.g. <c>&lt;button role="button"&gt;</c>, <c>&lt;nav role="navigation"&gt;</c>). This is
/// harmless but redundant in most cases - some authors add it deliberately for older assistive
/// technology compatibility or defensive coding, so removing it isn't always the right call.
/// Always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class RedundantRoleRule : IContentRule
{
    public string RuleId => "redundant-role";

    public string SuccessCriterion => "4.1.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<AriaSemanticsFragment>())
        {
            if (!fragment.IsRedundantRole)
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
                $"This <{fragment.TagName}> has role=\"{fragment.Role}\", which duplicates its native implicit role. It's harmless, but usually unnecessary - consider removing it unless it's needed for older assistive technology compatibility.");
        }
    }
}

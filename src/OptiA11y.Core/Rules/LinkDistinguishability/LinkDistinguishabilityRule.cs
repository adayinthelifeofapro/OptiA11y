using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LinkDistinguishability;

/// <summary>
/// Flags an inline link that sets an explicit <c>color</c> style and removes the underline
/// (<c>text-decoration: none</c>), leaving color as the only apparent visual cue distinguishing
/// it from surrounding body text (WCAG 1.4.1). Detecting both style declarations is
/// deterministic, but whether the link is truly indistinguishable depends on surrounding text
/// styling this rule cannot fully see, so it always reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LinkDistinguishabilityRule : IContentRule
{
    public string RuleId => "link-distinguishability";

    public string SuccessCriterion => "1.4.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (!link.HasExplicitColorStyle || !link.RemovesUnderline)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                link.Location,
                "This link is styled with an explicit color and no underline, which may leave color as the only cue distinguishing it from body text. Confirm it has an additional visual cue (underline, icon, weight), or add one (WCAG 1.4.1).");
        }
    }
}

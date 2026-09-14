using OptiA11y.Core.Model;

namespace OptiA11y.Core.Rules;

/// <summary>
/// Contract implemented by every content-analysis rule. Rules operate purely against an
/// <see cref="AuditDocument"/> — no CMS API, no DOM, no browser — which is what allows the
/// same rule to run against PaaS and SaaS content alike.
///
/// IMPORTANT: any rule whose judgement depends on natural-language quality, editorial intent,
/// or other heuristics MUST report findings with <see cref="Confidence.NeedsReview"/>, never
/// <see cref="Confidence.Fail"/>. <see cref="Confidence.Fail"/> is reserved for deterministic,
/// unambiguous violations of <see cref="SuccessCriterion"/> (e.g. a missing alt attribute, a
/// skipped heading level). This distinction is the mechanism that stops OptiA11y from
/// manufacturing false assurance, and it must not be weakened by individual rule implementations.
/// </summary>
public interface IContentRule
{
    /// <summary>A stable, unique identifier for this rule, e.g. "alt-text-quality".</summary>
    string RuleId { get; }

    /// <summary>The WCAG success criterion this rule evaluates, e.g. "1.1.1".</summary>
    string SuccessCriterion { get; }

    /// <summary>The WCAG conformance level of <see cref="SuccessCriterion"/>.</summary>
    WcagLevel Level { get; }

    /// <summary>
    /// Evaluates the rule against the supplied document and yields zero or more findings.
    /// Implementations must not throw for content that is merely unusual; unexpected shapes
    /// should be treated as "nothing to flag" rather than an error.
    /// </summary>
    IEnumerable<Finding> Evaluate(AuditDocument document);
}

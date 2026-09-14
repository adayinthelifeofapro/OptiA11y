namespace OptiA11y.Core.Model;

/// <summary>
/// A single observation produced by a rule against one fragment of an <see cref="AuditDocument"/>.
/// </summary>
/// <param name="RuleId">The stable identifier of the rule that produced this finding, e.g. "alt-text-quality".</param>
/// <param name="SuccessCriterion">The WCAG success criterion this finding relates to, e.g. "1.1.1".</param>
/// <param name="Level">The WCAG conformance level of <paramref name="SuccessCriterion"/>.</param>
/// <param name="Severity">The editorial impact of this finding.</param>
/// <param name="Confidence">
/// How certain this finding is. <see cref="Model.Confidence.NeedsReview"/> must be used for any
/// heuristic or natural-language judgement; <see cref="Model.Confidence.Fail"/> is reserved for
/// deterministic violations only.
/// </param>
/// <param name="Location">Where in the content this finding applies, for deep linking back to the offending property.</param>
/// <param name="Message">A human-readable, editor-facing explanation of the finding.</param>
public sealed record Finding(
    string RuleId,
    string SuccessCriterion,
    WcagLevel Level,
    Severity Severity,
    Confidence Confidence,
    SourceLocation Location,
    string Message);

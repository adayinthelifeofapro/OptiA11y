namespace OptiA11y.Core.Model;

/// <summary>
/// WCAG conformance level a rule's success criterion belongs to.
/// </summary>
public enum WcagLevel
{
    A,
    AA,
    AAA
}

/// <summary>
/// How serious a finding is from an editorial perspective. Distinct from <see cref="Confidence"/>:
/// severity is about impact, confidence is about certainty.
/// </summary>
public enum Severity
{
    Info,
    Minor,
    Major,
    Critical
}

/// <summary>
/// How certain the rule is that a finding represents a genuine accessibility problem.
/// This is the honesty mechanism at the centre of OptiA11y: heuristic rules (e.g. alt text
/// quality judgements) MUST report <see cref="NeedsReview"/> rather than <see cref="Fail"/>,
/// because the tool cannot know editorial intent. Only deterministic, unambiguous violations
/// (a missing alt attribute, a skipped heading level) may report <see cref="Fail"/>.
/// </summary>
public enum Confidence
{
    /// <summary>The fragment passes this rule's check.</summary>
    Pass,

    /// <summary>
    /// A heuristic judgement flagged something worth an editor's attention, but it is not
    /// a certain violation. This is the default for anything involving natural-language quality.
    /// </summary>
    NeedsReview,

    /// <summary>A deterministic, unambiguous violation of the success criterion.</summary>
    Fail
}

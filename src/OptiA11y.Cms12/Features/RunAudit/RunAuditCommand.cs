using OptiA11y.Core.Model;

namespace OptiA11y.Cms12.Features.RunAudit;

/// <summary>
/// Request to run a content accessibility audit for a single content item.
/// </summary>
/// <param name="ContentReference">The identifier of the content item to audit.</param>
public sealed record RunAuditCommand(string ContentReference);

/// <summary>
/// Result of running an audit: the findings produced, plus resolved editor links
/// where available.
/// </summary>
/// <param name="ContentReference">The content item that was audited.</param>
/// <param name="Findings">All findings produced by the rule engine, in deterministic order.</param>
/// <param name="EditorLinks">Resolved editor links keyed by the finding's location path string, where a link could be resolved.</param>
public sealed record RunAuditResult(
    string ContentReference,
    IReadOnlyList<Finding> Findings,
    IReadOnlyDictionary<string, string> EditorLinks);

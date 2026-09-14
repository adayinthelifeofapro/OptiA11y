using OptiA11y.Core.Model;

namespace OptiA11y.Core.Rules;

/// <summary>
/// Runs a collection of <see cref="IContentRule"/> instances against an <see cref="AuditDocument"/>
/// and aggregates their findings. Has no Optimizely dependency and no knowledge of how the
/// document was produced, so it serves both PaaS and SaaS adapters identically.
/// </summary>
public sealed class RuleEngine
{
    private readonly IReadOnlyList<IContentRule> _rules;

    public RuleEngine(IEnumerable<IContentRule> rules)
    {
        _rules = rules.ToList();
    }

    /// <summary>
    /// Evaluates every configured rule against the document and returns findings ordered
    /// deterministically by location, so results are stable across runs regardless of rule order.
    /// </summary>
    public IReadOnlyList<Finding> Audit(AuditDocument document)
    {
        var findings = new List<Finding>();

        foreach (var rule in _rules)
        {
            findings.AddRange(rule.Evaluate(document));
        }

        return findings
            .OrderBy(f => f.Location.ContentReference, StringComparer.Ordinal)
            .ThenBy(f => f.Location.PropertyName, StringComparer.Ordinal)
            .ThenBy(f => f.Location.ToPathString(), StringComparer.Ordinal)
            .ThenBy(f => f.Location.Ordinal)
            .ThenBy(f => f.RuleId, StringComparer.Ordinal)
            .ToList();
    }
}

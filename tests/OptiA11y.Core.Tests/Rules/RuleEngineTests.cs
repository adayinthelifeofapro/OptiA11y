using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class RuleEngineTests
{
    private sealed class StubRule : IContentRule
    {
        private readonly Func<AuditDocument, IEnumerable<Finding>> _evaluate;

        public StubRule(string ruleId, Func<AuditDocument, IEnumerable<Finding>> evaluate)
        {
            RuleId = ruleId;
            _evaluate = evaluate;
        }

        public string RuleId { get; }
        public string SuccessCriterion => "1.1.1";
        public WcagLevel Level => WcagLevel.A;

        public IEnumerable<Finding> Evaluate(AuditDocument document) => _evaluate(document);
    }

    [Fact]
    public void Audit_AggregatesFindingsFromAllRules()
    {
        var ruleA = new StubRule("rule-a", _ => new[]
        {
            new Finding("rule-a", "1.1.1", WcagLevel.A, Severity.Minor, Confidence.NeedsReview, TestLocations.OnMainBody(1), "a")
        });
        var ruleB = new StubRule("rule-b", _ => new[]
        {
            new Finding("rule-b", "1.1.1", WcagLevel.A, Severity.Minor, Confidence.NeedsReview, TestLocations.OnMainBody(0), "b")
        });

        var engine = new RuleEngine(new IContentRule[] { ruleA, ruleB });
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        var findings = engine.Audit(document);

        Assert.Equal(2, findings.Count);
    }

    [Fact]
    public void Audit_OrdersFindingsDeterministicallyByLocation()
    {
        var rule = new StubRule("rule-a", _ => new[]
        {
            new Finding("rule-a", "1.1.1", WcagLevel.A, Severity.Minor, Confidence.NeedsReview, TestLocations.OnMainBody(2), "third"),
            new Finding("rule-a", "1.1.1", WcagLevel.A, Severity.Minor, Confidence.NeedsReview, TestLocations.OnMainBody(0), "first"),
            new Finding("rule-a", "1.1.1", WcagLevel.A, Severity.Minor, Confidence.NeedsReview, TestLocations.OnMainBody(1), "second"),
        });

        var engine = new RuleEngine(new IContentRule[] { rule });
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        var findings = engine.Audit(document);

        Assert.Equal(new[] { "first", "second", "third" }, findings.Select(f => f.Message));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.DuplicateId;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class DuplicateIdRuleTests
{
    private readonly DuplicateIdRule _rule = new();

    [Fact]
    public void DuplicateId_IsFail()
    {
        var duplicate = new DuplicateIdFragment(TestLocations.OnMainBody(), "promo-heading", OccurrenceCount: 2);
        var document = new AuditDocument("content-1", new[] { duplicate });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NoDuplicates_ProducesNoFinding()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

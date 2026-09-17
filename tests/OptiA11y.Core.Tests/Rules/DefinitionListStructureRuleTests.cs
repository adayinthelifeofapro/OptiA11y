using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.DefinitionListStructure;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class DefinitionListStructureRuleTests
{
    private readonly DefinitionListStructureRule _rule = new();

    [Fact]
    public void DescriptionBeforeTerm_IsFail()
    {
        var list = new DefinitionListFragment(TestLocations.OnMainBody(), HasOrphanedTerm: false, DescriptionBeforeTerm: true);
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void OrphanedTerm_IsFail()
    {
        var list = new DefinitionListFragment(TestLocations.OnMainBody(), HasOrphanedTerm: true, DescriptionBeforeTerm: false);
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void WellFormedList_ProducesNoFinding()
    {
        var list = new DefinitionListFragment(TestLocations.OnMainBody(), HasOrphanedTerm: false, DescriptionBeforeTerm: false);
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

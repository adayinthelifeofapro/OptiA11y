using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ListMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ListMisuseRuleTests
{
    private readonly ListMisuseRule _rule = new();

    [Fact]
    public void EmptyList_IsFail()
    {
        var list = new ListStructureFragment(TestLocations.OnMainBody(), "list", ItemCount: 0, SampleText: "");
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void NonListItemChild_IsFail()
    {
        var list = new ListStructureFragment(TestLocations.OnMainBody(), "list", ItemCount: 2, SampleText: "a b", HasNonListItemChild: true);
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void WellFormedList_ProducesNoFinding()
    {
        var list = new ListStructureFragment(TestLocations.OnMainBody(), "list", ItemCount: 3, SampleText: "a b c");
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void FakeListKind_IsIgnored()
    {
        var list = new ListStructureFragment(TestLocations.OnMainBody(), "fake-list", ItemCount: 0, SampleText: "");
        var document = new AuditDocument("content-1", new[] { list });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

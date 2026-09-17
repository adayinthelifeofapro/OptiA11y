using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LayoutTable;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LayoutTableRuleTests
{
    private readonly LayoutTableRule _rule = new();

    [Fact]
    public void NoHeadersNoCaption_MultiColumn_IsNeedsReview()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: false, HasCaption: false, ColumnCount: 3);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void HasHeaders_ProducesNoFinding()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: false, ColumnCount: 3);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void SingleColumn_ProducesNoFinding()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: false, HasCaption: false, ColumnCount: 1);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

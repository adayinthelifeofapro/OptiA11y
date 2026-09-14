using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.TableComplexity;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class TableComplexityRuleTests
{
    private readonly TableComplexityRule _rule = new();

    [Fact]
    public void NoTables_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void MergedCellsWithoutScope_IsFail()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: true, ColumnCount: 3, HasMergedCells: true, AllHeaderCellsHaveScope: false);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void InconsistentRowLengthsWithoutMergedCells_IsFail()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: true, ColumnCount: 3, HasMergedCells: false, RowLengthsConsistent: false);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void MergedCellsWithScope_ProducesNoFindings()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: true, ColumnCount: 3, HasMergedCells: true, AllHeaderCellsHaveScope: true, RowLengthsConsistent: true);
        var document = new AuditDocument("content-1", new[] { table });

        Assert.Empty(_rule.Evaluate(document));
    }
}

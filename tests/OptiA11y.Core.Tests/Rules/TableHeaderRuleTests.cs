using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.TableHeaders;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class TableHeaderRuleTests
{
    private readonly TableHeaderRule _rule = new();

    [Fact]
    public void NoTables_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void TableWithoutHeaderRow_IsFail()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: false, HasCaption: true, ColumnCount: 3);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void TableWithoutCaption_IsNeedsReview()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: false, ColumnCount: 3);
        var document = new AuditDocument("content-1", new[] { table });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void WellFormedTable_ProducesNoFindings()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: true, HasCaption: true, ColumnCount: 3);
        var document = new AuditDocument("content-1", new[] { table });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void EmptyTable_DoesNotFlagMissingHeaderRow()
    {
        var table = new TableFragment(TestLocations.OnMainBody(), HasHeaderRow: false, HasCaption: true, ColumnCount: 0);
        var document = new AuditDocument("content-1", new[] { table });

        Assert.Empty(_rule.Evaluate(document));
    }
}

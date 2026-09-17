using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SameDestinationDifferentText;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SameDestinationDifferentTextRuleTests
{
    private readonly SameDestinationDifferentTextRule _rule = new();

    [Fact]
    public void SameDestinationDifferentText_FlagsBothLinks()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/report.pdf", "Read the report", IsDocumentLink: true),
            new LinkFragment(TestLocations.OnMainBody(1), "/report.pdf", "Download PDF", IsDocumentLink: true),
        };
        var document = new AuditDocument("content-1", links);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Equal(2, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.NeedsReview, f.Confidence));
    }

    [Fact]
    public void SameDestinationSameText_ProducesNoFindings()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/report.pdf", "Annual report", IsDocumentLink: true),
            new LinkFragment(TestLocations.OnMainBody(1), "/report.pdf", "Annual report", IsDocumentLink: true),
        };
        var document = new AuditDocument("content-1", links);

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoLinks_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }
}

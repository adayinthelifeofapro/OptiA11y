using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.DocumentLinkExpectations;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class DocumentLinkExpectationsRuleTests
{
    private readonly DocumentLinkExpectationsRule _rule = new();

    [Fact]
    public void NoLinks_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DocumentLinkWithoutFormatHint_IsNeedsReview()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/report.pdf", "Annual report", IsDocumentLink: true);
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DocumentLinkMentioningFormat_ProducesNoFindings()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/report.pdf", "Annual report (PDF)", IsDocumentLink: true);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NonDocumentLink_ProducesNoFindings()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/about", "About us", IsDocumentLink: false);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }
}

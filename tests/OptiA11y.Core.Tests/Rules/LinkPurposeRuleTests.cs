using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.LinkPurpose;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class LinkPurposeRuleTests
{
    private readonly LinkPurposeRule _rule = new();

    [Theory]
    [InlineData("click here")]
    [InlineData("Read more")]
    [InlineData("here")]
    public void NonDescriptiveText_IsNeedsReview(string text)
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/page", text, IsDocumentLink: false);
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void BareUrlAsText_IsNeedsReview()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "https://example.com/page", "https://example.com/page", IsDocumentLink: false);
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SameTextDifferentTargets_FlagsBothLinks()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/report-2023", "Annual report", IsDocumentLink: false),
            new LinkFragment(TestLocations.OnMainBody(1), "/report-2024", "Annual report", IsDocumentLink: false),
        };
        var document = new AuditDocument("content-1", links);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Equal(2, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.NeedsReview, f.Confidence));
    }

    [Fact]
    public void DescriptiveText_ProducesNoFindings()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/annual-report-2024", "Download the 2024 annual report", IsDocumentLink: true);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }
}

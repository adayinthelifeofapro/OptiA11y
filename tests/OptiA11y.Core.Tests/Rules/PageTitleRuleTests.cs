using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.PageTitle;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class PageTitleRuleTests
{
    private readonly PageTitleRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void MissingName_IsFail()
    {
        var metadata = new PageMetadataFragment(TestLocations.OnMainBody(), DisplayName: null);
        var document = new AuditDocument("content-1", new[] { metadata });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void EmptyName_IsFail()
    {
        var metadata = new PageMetadataFragment(TestLocations.OnMainBody(), DisplayName: "   ");
        var document = new AuditDocument("content-1", new[] { metadata });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void PlaceholderName_IsNeedsReview()
    {
        var metadata = new PageMetadataFragment(TestLocations.OnMainBody(), DisplayName: "New Page");
        var document = new AuditDocument("content-1", new[] { metadata });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void DescriptiveName_ProducesNoFindings()
    {
        var metadata = new PageMetadataFragment(TestLocations.OnMainBody(), DisplayName: "Contact us");
        var document = new AuditDocument("content-1", new[] { metadata });

        Assert.Empty(_rule.Evaluate(document));
    }
}

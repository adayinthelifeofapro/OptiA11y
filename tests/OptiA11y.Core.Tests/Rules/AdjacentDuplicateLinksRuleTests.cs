using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AdjacentDuplicateLinks;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AdjacentDuplicateLinksRuleTests
{
    private readonly AdjacentDuplicateLinksRule _rule = new();

    [Fact]
    public void ImageLinkFollowedByTextLinkToSameTarget_IsNeedsReview()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/product/kayak", string.Empty, IsDocumentLink: false),
            new LinkFragment(TestLocations.OnMainBody(1), "/product/kayak", "Blue Ridge Kayak", IsDocumentLink: false),
        };
        var document = new AuditDocument("content-1", links);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void BothLinksHaveText_ProducesNoFinding()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/product/kayak", "Kayak photo", IsDocumentLink: false),
            new LinkFragment(TestLocations.OnMainBody(1), "/product/kayak", "Blue Ridge Kayak", IsDocumentLink: false),
        };
        var document = new AuditDocument("content-1", links);

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void DifferentDestinations_ProducesNoFinding()
    {
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/product/kayak", string.Empty, IsDocumentLink: false),
            new LinkFragment(TestLocations.OnMainBody(1), "/product/paddle", "Blue Ridge Paddle", IsDocumentLink: false),
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

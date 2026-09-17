using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.ImageOfText;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class ImageOfTextRuleTests
{
    private readonly ImageOfTextRule _rule = new();

    [Fact]
    public void SentenceLikeAltText_IsNeedsReview()
    {
        var image = new ImageFragment(
            TestLocations.OnMainBody(),
            "quote-graphic.png",
            "Success is not final, failure is not fatal: it is the courage to continue that counts.",
            IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ShortDescriptiveAltText_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", "A red kayak on a calm lake", IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void MissingAltText_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", AltText: null, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

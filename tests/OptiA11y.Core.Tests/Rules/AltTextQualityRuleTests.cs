using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AltTextQuality;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AltTextQualityRuleTests
{
    private readonly AltTextQualityRule _rule = new();

    [Fact]
    public void MissingAltAttribute_IsDeterministicFail()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "photo.jpg", AltText: null, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void EmptyAltOnDecorativeCandidate_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "spacer.png", AltText: "", IsDecorativeCandidate: true);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }

    [Fact]
    public void EmptyAltOnNonDecorativeImage_IsNeedsReview()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "team-photo.jpg", AltText: "", IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Theory]
    [InlineData("IMG_2093.jpg")]
    [InlineData("team-photo.png")]
    public void FilenameLikeAltText_IsNeedsReview(string alt)
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", alt, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Theory]
    [InlineData("Image of a sunset over the harbour")]
    [InlineData("photo of the team at the conference")]
    public void RedundantPhrasing_IsNeedsReview(string alt)
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", alt, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ExcessivelyLongAltText_IsNeedsReview()
    {
        var alt = new string('a', 200);
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", alt, IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void GoodAltText_ProducesNoFinding()
    {
        var image = new ImageFragment(TestLocations.OnMainBody(), "src.jpg", "A red kayak on a calm lake at sunrise", IsDecorativeCandidate: false);
        var document = new AuditDocument("content-1", new[] { image });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Empty(findings);
    }
}

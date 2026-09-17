using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.EmojiOveruse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class EmojiOveruseRuleTests
{
    private readonly EmojiOveruseRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void RepeatedEmojiRun_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Great news \U0001F389\U0001F389\U0001F389", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void LeadingEmojiBullet_IsNeedsReview()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "\U0001F389 Party this Friday", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void SingleEmoji_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Great news \U0001F389 today", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

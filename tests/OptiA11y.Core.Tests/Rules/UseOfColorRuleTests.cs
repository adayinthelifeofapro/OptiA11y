using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.UseOfColor;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class UseOfColorRuleTests
{
    private readonly UseOfColorRule _rule = new();

    [Fact]
    public void NoText_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void TextReferencingColorAlone_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Click the red button to continue.", null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void UnrelatedText_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Welcome to our site.", null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }
}

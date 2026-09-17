using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.UnicodeStyledText;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class UnicodeStyledTextRuleTests
{
    private readonly UnicodeStyledTextRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void MathematicalAlphanumericText_IsFail()
    {
        // 𝗕𝗼𝗹𝗱 uses Mathematical Sans-Serif Bold code points.
        var fragment = new TextFragment(TestLocations.OnMainBody(), "𝗕𝗼𝗹𝗱 announcement", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void PlainText_ProducesNoFindings()
    {
        var fragment = new TextFragment(TestLocations.OnMainBody(), "Regular announcement", null);
        var document = new AuditDocument("content-1", new[] { fragment });

        Assert.Empty(_rule.Evaluate(document));
    }
}

using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.SensoryCharacteristics;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class SensoryCharacteristicsRuleTests
{
    private readonly SensoryCharacteristicsRule _rule = new();

    [Fact]
    public void NoText_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void PositionalInstruction_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Click the button on the right to continue.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ColorBasedInstruction_IsNeedsReview()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Press the green button to submit.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void OrdinaryText_ProducesNoFindings()
    {
        var text = new TextFragment(TestLocations.OnMainBody(), "Select the Submit button to continue.", LanguageCode: null);
        var document = new AuditDocument("content-1", new[] { text });

        Assert.Empty(_rule.Evaluate(document));
    }
}

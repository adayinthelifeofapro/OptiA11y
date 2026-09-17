using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.AbbreviationExpansion;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class AbbreviationExpansionRuleTests
{
    private readonly AbbreviationExpansionRule _rule = new();

    [Fact]
    public void NoFragments_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void RepeatedUnexpandedAcronym_IsNeedsReview()
    {
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Check our API for details.", null),
            new TextFragment(TestLocations.OnMainBody(1), "The API is documented online.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ExpandedAcronym_ProducesNoFindings()
    {
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Application Programming Interface (API) details.", null),
            new TextFragment(TestLocations.OnMainBody(1), "The API is documented online.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SingleOccurrence_ProducesNoFindings()
    {
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Check our API for details.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        Assert.Empty(_rule.Evaluate(document));
    }
}

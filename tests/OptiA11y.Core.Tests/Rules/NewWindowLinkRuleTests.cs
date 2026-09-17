using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.NewWindowLink;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class NewWindowLinkRuleTests
{
    private readonly NewWindowLinkRule _rule = new();

    [Fact]
    public void BlankTargetWithNoWarning_IsNeedsReview()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "https://partner.example.com", "Visit our partner", IsDocumentLink: false, Target: "_blank");
        var document = new AuditDocument("content-1", new[] { link });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void BlankTargetWithWarningInText_ProducesNoFinding()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "https://partner.example.com", "Visit our partner (opens in a new window)", IsDocumentLink: false, Target: "_blank");
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void SelfTarget_ProducesNoFinding()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/page", "Visit our page", IsDocumentLink: false, Target: "_self");
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void NoTarget_ProducesNoFinding()
    {
        var link = new LinkFragment(TestLocations.OnMainBody(), "/page", "Visit our page", IsDocumentLink: false);
        var document = new AuditDocument("content-1", new[] { link });

        Assert.Empty(_rule.Evaluate(document));
    }
}

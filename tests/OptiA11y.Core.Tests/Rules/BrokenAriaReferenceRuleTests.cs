using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.BrokenAriaReference;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class BrokenAriaReferenceRuleTests
{
    private readonly BrokenAriaReferenceRule _rule = new();

    [Fact]
    public void NoReferences_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void UnresolvedReference_IsNeedsReview()
    {
        var reference = new AriaReferenceFragment(TestLocations.OnMainBody(), "aria-labelledby", "missing-id", ResolvedWithinSameFragment: false);
        var document = new AuditDocument("content-1", new[] { reference });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.NeedsReview, findings[0].Confidence);
    }

    [Fact]
    public void ResolvedReference_ProducesNoFindings()
    {
        var reference = new AriaReferenceFragment(TestLocations.OnMainBody(), "aria-labelledby", "present-id", ResolvedWithinSameFragment: true);
        var document = new AuditDocument("content-1", new[] { reference });

        Assert.Empty(_rule.Evaluate(document));
    }
}

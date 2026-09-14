using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules.HeadingStructure;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

public sealed class HeadingStructureRuleTests
{
    private readonly HeadingStructureRule _rule = new();

    [Fact]
    public void NoHeadings_ProducesNoFindings()
    {
        var document = new AuditDocument("content-1", Array.Empty<ContentFragment>());

        Assert.Empty(_rule.Evaluate(document));
    }

    [Fact]
    public void EmptyHeading_IsFail()
    {
        var heading = new HeadingFragment(TestLocations.OnMainBody(), 1, "   ");
        var document = new AuditDocument("content-1", new[] { heading });

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void MultipleH1s_FlagsAllButTheFirst()
    {
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 1, "First"),
            new HeadingFragment(TestLocations.OnMainBody(1), 1, "Second"),
            new HeadingFragment(TestLocations.OnMainBody(2), 1, "Third"),
        };
        var document = new AuditDocument("content-1", headings);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Equal(2, findings.Count);
        Assert.All(findings, f => Assert.Equal(Confidence.Fail, f.Confidence));
    }

    [Fact]
    public void SkippedLevel_IsFail()
    {
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 1, "Title"),
            new HeadingFragment(TestLocations.OnMainBody(1), 3, "Subsection"),
        };
        var document = new AuditDocument("content-1", headings);

        var findings = _rule.Evaluate(document).ToList();

        Assert.Single(findings);
        Assert.Equal(Confidence.Fail, findings[0].Confidence);
    }

    [Fact]
    public void WellFormedHeadings_ProduceNoFindings()
    {
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 1, "Title"),
            new HeadingFragment(TestLocations.OnMainBody(1), 2, "Section"),
            new HeadingFragment(TestLocations.OnMainBody(2), 3, "Subsection"),
        };
        var document = new AuditDocument("content-1", headings);

        Assert.Empty(_rule.Evaluate(document));
    }
}

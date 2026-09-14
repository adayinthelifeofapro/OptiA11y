using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.AudioDescription;
using OptiA11y.Core.Rules.BrokenAriaReference;
using OptiA11y.Core.Rules.ColorContrast;
using OptiA11y.Core.Rules.DocumentLinkExpectations;
using OptiA11y.Core.Rules.FauxHeading;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.InputPurpose;
using OptiA11y.Core.Rules.LanguageOfParts;
using OptiA11y.Core.Rules.LinkPurpose;
using OptiA11y.Core.Rules.Motion;
using OptiA11y.Core.Rules.PageTitle;
using OptiA11y.Core.Rules.ReadingLevel;
using OptiA11y.Core.Rules.Reflow;
using OptiA11y.Core.Rules.SensoryCharacteristics;
using OptiA11y.Core.Rules.TextReadability;
using OptiA11y.Core.Rules.TitleAttributeMisuse;
using Xunit;

namespace OptiA11y.Core.Tests.Rules;

/// <summary>
/// Enforces the honesty mechanism at the centre of OptiA11y: any rule whose logic is a
/// heuristic judgement (not a deterministic structural fact) must never report
/// <see cref="Confidence.Fail"/>. Each test below exercises the heuristic branch(es) of a rule
/// against inputs designed to trigger them, and asserts none of them ever produce a Fail.
/// HeadingStructure's skipped-level/multiple-H1/empty-heading checks, and ColorContrast's normal
/// ratio check, are deterministic structural facts and are permitted to report Fail - the tests
/// below for those two rules deliberately exercise ONLY their heuristic branches (missing-H1,
/// overlong heading, and image-background contrast respectively) in isolation.
/// </summary>
public sealed class HeuristicRulesNeverFailTests
{
    [Fact]
    public void AltTextQualityRule_NeverReportsFail_ForHeuristicJudgements()
    {
        var rule = new AltTextQualityRule();
        var images = new ContentFragment[]
        {
            new ImageFragment(TestLocations.OnMainBody(0), "src.jpg", "IMG_001.jpg", false),
            new ImageFragment(TestLocations.OnMainBody(1), "src.jpg", "image of a mountain range", false),
            new ImageFragment(TestLocations.OnMainBody(2), "src.jpg", new string('x', 300), false),
            new ImageFragment(TestLocations.OnMainBody(3), "src.jpg", "", false),
        };
        var document = new AuditDocument("content-1", images);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LinkPurposeRule_NeverReportsFail_ForHeuristicJudgements()
    {
        var rule = new LinkPurposeRule();
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/a", "click here", false),
            new LinkFragment(TestLocations.OnMainBody(1), "https://example.com", "https://example.com", false),
            new LinkFragment(TestLocations.OnMainBody(2), "", "", false),
            new LinkFragment(TestLocations.OnMainBody(3), "/x", "Report", false),
            new LinkFragment(TestLocations.OnMainBody(4), "/y", "Report", false),
        };
        var document = new AuditDocument("content-1", links);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HeadingStructureRule_MayReportFail_BecauseItsChecksAreDeterministic()
    {
        // Documents the deliberate exception: structural facts, not heuristics, may Fail.
        var rule = new HeadingStructureRule();
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 1, ""),
        };
        var document = new AuditDocument("content-1", headings);

        var findings = rule.Evaluate(document).ToList();

        Assert.Contains(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HeadingStructureRule_NeverReportsFail_ForItsHeuristicChecks()
    {
        // Isolates the missing-H1 and overlong-heading checks, which are judgement calls
        // (a CMS page's H1 may legitimately come from the template) rather than structural facts.
        var rule = new HeadingStructureRule();
        var headings = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 2, new string('x', 150)),
        };
        var document = new AuditDocument("content-1", headings);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void TextReadabilityRule_NeverReportsFail_ForItsShoutingCheck()
    {
        // Isolates the all-caps "shouting" check, which is a judgement call (emphasis vs. an
        // acronym vs. genuinely shouted text), from the deterministic justified/tiny-font checks.
        var rule = new TextReadabilityRule();
        var text = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "PLEASE READ THIS IMPORTANT NOTICE carefully.", null),
        };
        var document = new AuditDocument("content-1", text);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ColorContrastRule_NeverReportsFail_ForImageBackgroundContrast()
    {
        // A background sampled from behind an image/gradient is unreliable, so even a
        // ratio well below threshold must not be a Fail.
        var rule = new ColorContrastRule();
        var fragments = new ContentFragment[]
        {
            new ColorContrastFragment(TestLocations.OnMainBody(0), "#777", "#666", 1.1, false, "Sample text", BackgroundIsImage: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void PageTitleRule_NeverReportsFail_ForPlaceholderNameCheck()
    {
        // A non-empty but generic name is a judgement call; only a genuinely empty name is a
        // deterministic Fail (covered elsewhere).
        var rule = new PageTitleRule();
        var fragments = new ContentFragment[]
        {
            new PageMetadataFragment(TestLocations.OnMainBody(0), "New Page"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void DocumentLinkExpectationsRule_NeverReportsFail()
    {
        var rule = new DocumentLinkExpectationsRule();
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/report.pdf", "Annual report", true),
        };
        var document = new AuditDocument("content-1", links);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LanguageOfPartsRule_NeverReportsFail()
    {
        var rule = new LanguageOfPartsRule();
        var text = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Please say добро пожаловать to our guests.", null),
        };
        var document = new AuditDocument("content-1", text);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SensoryCharacteristicsRule_NeverReportsFail()
    {
        var rule = new SensoryCharacteristicsRule();
        var text = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Click the button on the right to continue.", null),
        };
        var document = new AuditDocument("content-1", text);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ReadingLevelRule_NeverReportsFail()
    {
        var rule = new ReadingLevelRule();
        const string difficultPassage =
            "Notwithstanding the aforementioned considerations, the organizational infrastructure " +
            "necessitates a comprehensive reevaluation of preexisting methodological frameworks in " +
            "order to accommodate the multifaceted, interdisciplinary requirements engendered by " +
            "the unprecedented technological transformations currently permeating substantially all " +
            "operational departments within the multinational corporate conglomerate.";
        var text = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), difficultPassage, null),
        };
        var document = new AuditDocument("content-1", text);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FauxHeadingRule_NeverReportsFail()
    {
        var rule = new FauxHeadingRule();
        var fragments = new ContentFragment[]
        {
            new EmphasisBlockFragment(TestLocations.OnMainBody(0), "Getting started", 15),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void AudioDescriptionRule_NeverReportsFail()
    {
        var rule = new AudioDescriptionRule();
        var media = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "video.mp4", "video", true),
        };
        var document = new AuditDocument("content-1", media);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void BrokenAriaReferenceRule_NeverReportsFail()
    {
        var rule = new BrokenAriaReferenceRule();
        var fragments = new ContentFragment[]
        {
            new AriaReferenceFragment(TestLocations.OnMainBody(0), "aria-labelledby", "missing-id", false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void InputPurposeRule_NeverReportsFail()
    {
        var rule = new InputPurposeRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "email", true, null, "email"),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void TitleAttributeMisuseRule_NeverReportsFail()
    {
        var rule = new TitleAttributeMisuseRule();
        var links = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/about", "About us", false, true, "About us"),
        };
        var document = new AuditDocument("content-1", links);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void MotionRule_NeverReportsFail()
    {
        var rule = new MotionRule();
        var fragments = new ContentFragment[]
        {
            new MotionFragment(TestLocations.OnMainBody(0), "<div class=\"banner-carousel\">"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ReflowRule_NeverReportsFail()
    {
        var rule = new ReflowRule();
        var fragments = new ContentFragment[]
        {
            new ReflowFragment(TestLocations.OnMainBody(0)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }
}

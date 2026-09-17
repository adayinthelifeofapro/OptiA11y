using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Rules;
using OptiA11y.Core.Rules.AdjacentDuplicateLinks;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.AudioDescription;
using OptiA11y.Core.Rules.BlockquoteMisuse;
using OptiA11y.Core.Rules.BrokenAriaReference;
using OptiA11y.Core.Rules.ColorContrast;
using OptiA11y.Core.Rules.ComplexImageDescription;
using OptiA11y.Core.Rules.DecorativeImageMisuse;
using OptiA11y.Core.Rules.DocumentLinkExpectations;
using OptiA11y.Core.Rules.EmphasisMisuse;
using OptiA11y.Core.Rules.ErrorIdentification;
using OptiA11y.Core.Rules.FigureCaptionMismatch;
using OptiA11y.Core.Rules.ImageOfText;
using OptiA11y.Core.Rules.FauxHeading;
using OptiA11y.Core.Rules.FormInstructions;
using OptiA11y.Core.Rules.HeadingLength;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.InputPurpose;
using OptiA11y.Core.Rules.InputTypeAppropriateness;
using OptiA11y.Core.Rules.LabelQuality;
using OptiA11y.Core.Rules.LanguageOfParts;
using OptiA11y.Core.Rules.LayoutTable;
using OptiA11y.Core.Rules.LinkPurpose;
using OptiA11y.Core.Rules.MeaningfulSequence;
using OptiA11y.Core.Rules.Motion;
using OptiA11y.Core.Rules.NewWindowLink;
using OptiA11y.Core.Rules.PageTitle;
using OptiA11y.Core.Rules.ReadingLevel;
using OptiA11y.Core.Rules.ReadonlyDisabledMisuse;
using OptiA11y.Core.Rules.RedundantEntry;
using OptiA11y.Core.Rules.Reflow;
using OptiA11y.Core.Rules.RequiredFieldIndication;
using OptiA11y.Core.Rules.SameDestinationDifferentText;
using OptiA11y.Core.Rules.SelectOptionQuality;
using OptiA11y.Core.Rules.SensoryCharacteristics;
using OptiA11y.Core.Rules.SkipLinkTarget;
using OptiA11y.Core.Rules.SuperscriptSubscriptMisuse;
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

    [Fact]
    public void ImageOfTextRule_NeverReportsFail()
    {
        var rule = new ImageOfTextRule();
        var images = new ContentFragment[]
        {
            new ImageFragment(TestLocations.OnMainBody(0), "quote.png", "Success is not final, failure is not fatal: it is the courage to continue that counts.", false),
        };
        var document = new AuditDocument("content-1", images);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void DecorativeImageMisuseRule_NeverReportsFail()
    {
        var rule = new DecorativeImageMisuseRule();
        var figures = new ContentFragment[]
        {
            new FigureFragment(TestLocations.OnMainBody(0), "", "Quarterly revenue by region"),
        };
        var document = new AuditDocument("content-1", figures);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FigureCaptionMismatchRule_NeverReportsFail()
    {
        var rule = new FigureCaptionMismatchRule();
        var figures = new ContentFragment[]
        {
            new FigureFragment(TestLocations.OnMainBody(0), "Team at the summit", "Team at the summit"),
        };
        var document = new AuditDocument("content-1", figures);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ComplexImageDescriptionRule_NeverReportsFail()
    {
        var rule = new ComplexImageDescriptionRule();
        var images = new ContentFragment[]
        {
            new ImageFragment(TestLocations.OnMainBody(0), "sales-chart.png", "Sales chart", false),
        };
        var document = new AuditDocument("content-1", images);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LayoutTableRule_NeverReportsFail()
    {
        var rule = new LayoutTableRule();
        var fragments = new ContentFragment[]
        {
            new TableFragment(TestLocations.OnMainBody(0), HasHeaderRow: false, HasCaption: false, ColumnCount: 3),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void BlockquoteMisuseRule_NeverReportsFail()
    {
        var rule = new BlockquoteMisuseRule();
        var fragments = new ContentFragment[]
        {
            new MarkupSpanFragment(TestLocations.OnMainBody(0), "blockquote", "All prices in USD", false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HeadingLengthRule_NeverReportsFail()
    {
        var rule = new HeadingLengthRule();
        var fragments = new ContentFragment[]
        {
            new HeadingFragment(TestLocations.OnMainBody(0), 2, new string('x', 150)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LabelQualityRule_NeverReportsFail()
    {
        var rule = new LabelQualityRule();
        var fragments = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "text", true, LabelText: "Field 1"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void EmphasisMisuseRule_NeverReportsFail()
    {
        var rule = new EmphasisMisuseRule();
        var fragments = new ContentFragment[]
        {
            new MarkupSpanFragment(TestLocations.OnMainBody(0), "b", new string('x', 130)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SuperscriptSubscriptMisuseRule_NeverReportsFail()
    {
        var rule = new SuperscriptSubscriptMisuseRule();
        var fragments = new ContentFragment[]
        {
            new MarkupSpanFragment(TestLocations.OnMainBody(0), "sup", "This entire clause is wrapped in superscript"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void MeaningfulSequenceRule_NeverReportsFail()
    {
        var rule = new MeaningfulSequenceRule();
        var fragments = new ContentFragment[]
        {
            new PositionedContentFragment(TestLocations.OnMainBody(0), "absolute", null, "Sidebar note"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SameDestinationDifferentTextRule_NeverReportsFail()
    {
        var rule = new SameDestinationDifferentTextRule();
        var fragments = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/report.pdf", "Read the report", IsDocumentLink: true),
            new LinkFragment(TestLocations.OnMainBody(1), "/report.pdf", "Download PDF", IsDocumentLink: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void NewWindowLinkRule_NeverReportsFail()
    {
        var rule = new NewWindowLinkRule();
        var fragments = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "https://partner.example.com", "Visit our partner", IsDocumentLink: false, Target: "_blank"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void AdjacentDuplicateLinksRule_NeverReportsFail()
    {
        var rule = new AdjacentDuplicateLinksRule();
        var fragments = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/product/kayak", string.Empty, IsDocumentLink: false),
            new LinkFragment(TestLocations.OnMainBody(1), "/product/kayak", "Blue Ridge Kayak", IsDocumentLink: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SkipLinkTargetRule_NeverReportsFail()
    {
        var rule = new SkipLinkTargetRule();
        var fragments = new ContentFragment[]
        {
            new SkipLinkFragment(TestLocations.OnMainBody(0), "main-content", ResolvedWithinSameFragment: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void RequiredFieldIndicationRule_NeverReportsFail()
    {
        var rule = new RequiredFieldIndicationRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "text", true, HasVisualRequiredIndicator: true, IsRequired: false),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ErrorIdentificationRule_NeverReportsFail()
    {
        var rule = new ErrorIdentificationRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "text", true, AriaInvalid: true, HasAriaDescribedBy: false),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FormInstructionsRule_NeverReportsFail()
    {
        var rule = new FormInstructionsRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "email", true, HasPatternOrFormatConstraint: true, HasAriaDescribedBy: false),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SelectOptionQualityRule_NeverReportsFail()
    {
        var rule = new SelectOptionQualityRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "select", null, true, FirstOptionText: "Please select"),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ReadonlyDisabledMisuseRule_NeverReportsFail()
    {
        var rule = new ReadonlyDisabledMisuseRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "text", true, IsDisabled: true, IsRequired: true),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void RedundantEntryRule_NeverReportsFail()
    {
        var rule = new RedundantEntryRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "email", true, LabelText: "Confirm email"),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void InputTypeAppropriatenessRule_NeverReportsFail()
    {
        var rule = new InputTypeAppropriatenessRule();
        var fields = new ContentFragment[]
        {
            new FormFieldFragment(TestLocations.OnMainBody(0), "input", "text", true, InferredPurposeCategory: "email"),
        };
        var document = new AuditDocument("content-1", fields);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void RedundantRoleRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.RedundantRole.RedundantRoleRule();
        var fragments = new ContentFragment[]
        {
            new AriaSemanticsFragment(
                TestLocations.OnMainBody(0), "button", "button",
                MissingRequiredOwnedElementDescription: null,
                MissingRequiredAttributes: Array.Empty<string>(),
                DisallowedWidgetStateAttributes: Array.Empty<string>(),
                IsAriaHiddenWithFocusableDescendant: false,
                IsFocusable: true,
                HasGlobalAriaAttribute: false,
                IsRedundantRole: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LandmarkStructureRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LandmarkStructure.LandmarkStructureRule();
        var fragments = new ContentFragment[]
        {
            new LandmarkFragment(TestLocations.OnMainBody(0), "main", "main", IsExplicitRole: false, HasAccessibleName: false),
            new LandmarkFragment(TestLocations.OnMainBody(1), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: false),
            new LandmarkFragment(TestLocations.OnMainBody(2), "nav", "navigation", IsExplicitRole: false, HasAccessibleName: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LiveRegionMisuseRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LiveRegionMisuse.LiveRegionMisuseRule();
        var fragments = new ContentFragment[]
        {
            new LiveRegionFragment(TestLocations.OnMainBody(0), "div", null, "eventually", HasInvalidPolitenessValue: true, TextLength: 10),
            new LiveRegionFragment(TestLocations.OnMainBody(1), "div", "status", "polite", HasInvalidPolitenessValue: false, TextLength: 500),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void AbbreviationExpansionRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.AbbreviationExpansion.AbbreviationExpansionRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Check our API for details.", null),
            new TextFragment(TestLocations.OnMainBody(1), "The API is documented online.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void UnusualWordsRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.UnusualWords.UnusualWordsRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "We need to leverage synergy across teams.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void PronunciationAmbiguityRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.PronunciationAmbiguity.PronunciationAmbiguityRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Please read the manual before use.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void EmojiOveruseRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.EmojiOveruse.EmojiOveruseRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Great news \U0001F389\U0001F389\U0001F389", null),
            new TextFragment(TestLocations.OnMainBody(1), "\U0001F389 Party this Friday", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void AsciiArtRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.AsciiArt.AsciiArtRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "----------------", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void WhitespaceFormattingRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.WhitespaceFormatting.WhitespaceFormattingRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Item\u00A0\u00A0\u00A0Price", null),
            new TextFragment(TestLocations.OnMainBody(1), "Item     Price", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LineBreakMisuseRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LineBreakMisuse.LineBreakMisuseRule();
        var fragments = new ContentFragment[]
        {
            new LineBreakRunFragment(TestLocations.OnMainBody(0), 3),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LinkTextLanguageRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LinkTextLanguage.LinkTextLanguageRule();
        var fragments = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/page", "Привет мир", IsDocumentLink: false, LanguageCode: null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void TimedContentRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.TimedContent.TimedContentRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Your session will time out in 5 minutes.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void MediaTranscriptQualityRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.MediaTranscriptQuality.MediaTranscriptQualityRule();
        var fragments = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "video.mp4", "video", true, TranscriptHref: "video.mp4", TranscriptLinkText: "transcript"),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void SignLanguageRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.SignLanguage.SignLanguageRule();
        var fragments = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "video.mp4", "video", true, HasSignLanguageTrack: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void ExtendedAudioDescriptionRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.ExtendedAudioDescription.ExtendedAudioDescriptionRule();
        var fragments = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "video.mp4", "video", true, HasDescriptionTrack: true, HasExtendedDescriptionTrack: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void MediaAlternativeRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.MediaAlternative.MediaAlternativeRule();
        var fragments = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "video.mp4", "video", HasCaptionsOrTranscript: false, HasDescriptionTrack: false),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FlashingContentRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.FlashingContent.FlashingContentRule();
        var fragments = new ContentFragment[]
        {
            new MediaFragment(TestLocations.OnMainBody(0), "flash-intro.mp4", "video", true, HasFlashIndicator: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void UseOfColorRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.UseOfColor.UseOfColorRule();
        var fragments = new ContentFragment[]
        {
            new TextFragment(TestLocations.OnMainBody(0), "Click the red button to continue.", null),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LinkDistinguishabilityRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LinkDistinguishability.LinkDistinguishabilityRule();
        var fragments = new ContentFragment[]
        {
            new LinkFragment(TestLocations.OnMainBody(0), "/page", "Learn more", IsDocumentLink: false, HasExplicitColorStyle: true, RemovesUnderline: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LineLengthRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LineLength.LineLengthRule();
        var fragments = new ContentFragment[]
        {
            new TextStyleFragment(TestLocations.OnMainBody(0), null, null, "Sample text", WidthPx: 1200),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LineSpacingRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LineSpacing.LineSpacingRule();
        var fragments = new ContentFragment[]
        {
            new TextStyleFragment(TestLocations.OnMainBody(0), null, null, "Sample text", LineHeight: 1.1),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void BackgroundImageTextRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.BackgroundImageText.BackgroundImageTextRule();
        var fragments = new ContentFragment[]
        {
            new TextStyleFragment(TestLocations.OnMainBody(0), null, null, "Sample text", HasBackgroundImage: true),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FocusOrderRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.FocusOrder.FocusOrderRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.FocusOrderFragment(TestLocations.OnMainBody(0)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FocusAppearanceRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.FocusAppearance.FocusAppearanceRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.FocusAppearanceFragment(TestLocations.OnMainBody(0), "<button> \"Submit\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HoverFocusContentRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.HoverFocusContent.HoverFocusContentRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.HoverFocusContentFragment(TestLocations.OnMainBody(0), "<a> \"Info\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void StickyObstructionRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.StickyObstruction.StickyObstructionRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.StickyObstructionFragment(TestLocations.OnMainBody(0)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void FlashThresholdRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.FlashThreshold.FlashThresholdRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.FlashThresholdFragment(TestLocations.OnMainBody(0)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void PointerTargetSpacingRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.PointerTargetSpacing.PointerTargetSpacingRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.PointerTargetSpacingFragment(TestLocations.OnMainBody(0), "<button> \"X\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void PointerGesturesRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.PointerGestures.PointerGesturesRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.PointerGesturesFragment(TestLocations.OnMainBody(0), "<div> \"Gallery\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void DraggingMovementsRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.DraggingMovements.DraggingMovementsRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.DraggingMovementsFragment(TestLocations.OnMainBody(0), "<div> \"Slider\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void StatusMessagesRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.StatusMessages.StatusMessagesRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.StatusMessagesFragment(TestLocations.OnMainBody(0), "<div> \"Saved\""),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void LandmarkCompletenessRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.LandmarkCompleteness.LandmarkCompletenessRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.LandmarkCompletenessFragment(TestLocations.OnMainBody(0), false, Array.Empty<string>(), Array.Empty<string>()),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }

    [Fact]
    public void HeadingInViewportOrderRule_NeverReportsFail()
    {
        var rule = new global::OptiA11y.Core.Rules.HeadingInViewportOrder.HeadingInViewportOrderRule();
        var fragments = new ContentFragment[]
        {
            new global::OptiA11y.Core.Model.Fragments.HeadingInViewportOrderFragment(TestLocations.OnMainBody(0)),
        };
        var document = new AuditDocument("content-1", fragments);

        var findings = rule.Evaluate(document).ToList();

        Assert.NotEmpty(findings);
        Assert.DoesNotContain(findings, f => f.Confidence == Confidence.Fail);
    }
}

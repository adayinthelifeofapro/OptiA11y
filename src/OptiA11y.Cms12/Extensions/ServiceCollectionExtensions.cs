using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OptiA11y.Cms12.Features.RunAudit;
using OptiA11y.Cms12.Infrastructure.ContentAdapter;
using OptiA11y.Core.Rules;
using OptiA11y.Core.Rules.AdjacentDuplicateLinks;
using OptiA11y.Core.Rules.AbbreviationExpansion;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.AriaAllowedAttribute;
using OptiA11y.Core.Rules.AriaHiddenFocusable;
using OptiA11y.Core.Rules.AriaRequiredAttributes;
using OptiA11y.Core.Rules.AriaRequiredChildren;
using OptiA11y.Core.Rules.AsciiArt;
using OptiA11y.Core.Rules.AudioDescription;
using OptiA11y.Core.Rules.AutoplayMedia;
using OptiA11y.Core.Rules.BackgroundImageText;
using OptiA11y.Core.Rules.BlinkingContent;
using OptiA11y.Core.Rules.BlockquoteMisuse;
using OptiA11y.Core.Rules.BrokenAriaReference;
using OptiA11y.Core.Rules.ButtonName;
using OptiA11y.Core.Rules.BypassBlocks;
using OptiA11y.Core.Rules.ColorContrast;
using OptiA11y.Core.Rules.ComplexImageDescription;
using OptiA11y.Core.Rules.ContrastEnhanced;
using OptiA11y.Core.Rules.DecorativeImageMisuse;
using OptiA11y.Core.Rules.DefinitionListStructure;
using OptiA11y.Core.Rules.DeprecatedElements;
using OptiA11y.Core.Rules.DocumentLinkExpectations;
using OptiA11y.Core.Rules.DraggingMovements;
using OptiA11y.Core.Rules.DuplicateId;
using OptiA11y.Core.Rules.EmphasisMisuse;
using OptiA11y.Core.Rules.EmojiOveruse;
using OptiA11y.Core.Rules.ErrorIdentification;
using OptiA11y.Core.Rules.ExtendedAudioDescription;
using OptiA11y.Core.Rules.FauxHeading;
using OptiA11y.Core.Rules.FieldsetLegend;
using OptiA11y.Core.Rules.FlashingContent;
using OptiA11y.Core.Rules.FocusIndicator;
using OptiA11y.Core.Rules.FormInstructions;
using OptiA11y.Core.Rules.FormLabels;
using OptiA11y.Core.Rules.FigureCaptionMismatch;
using OptiA11y.Core.Rules.FlashThreshold;
using OptiA11y.Core.Rules.FocusAppearance;
using OptiA11y.Core.Rules.FocusNotObscured;
using OptiA11y.Core.Rules.FocusOrder;
using OptiA11y.Core.Rules.HeadingInViewportOrder;
using OptiA11y.Core.Rules.HeadingLength;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.HoverFocusContent;
using OptiA11y.Core.Rules.IframeTitle;
using OptiA11y.Core.Rules.ImageOfText;
using OptiA11y.Core.Rules.InputPurpose;
using OptiA11y.Core.Rules.InputTypeAppropriateness;
using OptiA11y.Core.Rules.InteractiveAttributes;
using OptiA11y.Core.Rules.InvalidAria;
using OptiA11y.Core.Rules.KeyboardOperable;
using OptiA11y.Core.Rules.KeyboardTrap;
using OptiA11y.Core.Rules.LabelInName;
using OptiA11y.Core.Rules.LabelQuality;
using OptiA11y.Core.Rules.LandmarkCompleteness;
using OptiA11y.Core.Rules.LandmarkStructure;
using OptiA11y.Core.Rules.LanguageAttribute;
using OptiA11y.Core.Rules.LanguageOfParts;
using OptiA11y.Core.Rules.LayoutTable;
using OptiA11y.Core.Rules.LineLength;
using OptiA11y.Core.Rules.LineSpacing;
using OptiA11y.Core.Rules.LinkDistinguishability;
using OptiA11y.Core.Rules.LinkName;
using OptiA11y.Core.Rules.LinkPurpose;
using OptiA11y.Core.Rules.LinkTextLanguage;
using OptiA11y.Core.Rules.LineBreakMisuse;
using OptiA11y.Core.Rules.ListMisuse;
using OptiA11y.Core.Rules.ListStructure;
using OptiA11y.Core.Rules.LiveRegionMisuse;
using OptiA11y.Core.Rules.MeaningfulSequence;
using OptiA11y.Core.Rules.MediaAlternative;
using OptiA11y.Core.Rules.MediaCaptions;
using OptiA11y.Core.Rules.MediaTranscriptQuality;
using OptiA11y.Core.Rules.MetaRefresh;
using OptiA11y.Core.Rules.Motion;
using OptiA11y.Core.Rules.NestedInteractive;
using OptiA11y.Core.Rules.NewWindowLink;
using OptiA11y.Core.Rules.NonTextContrast;
using OptiA11y.Core.Rules.OrientationLock;
using OptiA11y.Core.Rules.PageTitle;
using OptiA11y.Core.Rules.PlaceholderAsLabel;
using OptiA11y.Core.Rules.PointerGestures;
using OptiA11y.Core.Rules.PointerTargetSpacing;
using OptiA11y.Core.Rules.PresentationRoleConflict;
using OptiA11y.Core.Rules.PronunciationAmbiguity;
using OptiA11y.Core.Rules.ReadingLevel;
using OptiA11y.Core.Rules.ReadonlyDisabledMisuse;
using OptiA11y.Core.Rules.RedundantEntry;
using OptiA11y.Core.Rules.RedundantRole;
using OptiA11y.Core.Rules.Reflow;
using OptiA11y.Core.Rules.RequiredFieldIndication;
using OptiA11y.Core.Rules.ResizeText;
using OptiA11y.Core.Rules.SameDestinationDifferentText;
using OptiA11y.Core.Rules.SelectOptionQuality;
using OptiA11y.Core.Rules.SensoryCharacteristics;
using OptiA11y.Core.Rules.SignLanguage;
using OptiA11y.Core.Rules.SkipLinkTarget;
using OptiA11y.Core.Rules.StatusMessages;
using OptiA11y.Core.Rules.StickyObstruction;
using OptiA11y.Core.Rules.SuperscriptSubscriptMisuse;
using OptiA11y.Core.Rules.SvgAccessibleName;
using OptiA11y.Core.Rules.TableComplexity;
using OptiA11y.Core.Rules.TableHeaders;
using OptiA11y.Core.Rules.TargetSize;
using OptiA11y.Core.Rules.TextReadability;
using OptiA11y.Core.Rules.TextSpacing;
using OptiA11y.Core.Rules.TimedContent;
using OptiA11y.Core.Rules.TitleAttributeMisuse;
using OptiA11y.Core.Rules.UnicodeStyledText;
using OptiA11y.Core.Rules.UnusualWords;
using OptiA11y.Core.Rules.UseOfColor;
using OptiA11y.Core.Rules.WhitespaceFormatting;
using OptiA11y.Rendering;

namespace OptiA11y.Cms12.Extensions;

/// <summary>
/// Registers OptiA11y's slice-one services: the rule set, the rule engine, the RunAudit
/// handler, and the PaaS content adapter. In a CMS 13 host this is called automatically by
/// <see cref="OptiA11y.Cms12.Infrastructure.OptiA11yCmsModule"/>, which also registers the real
/// <see cref="IPaasContentLoader"/> implementation backed by EPiServer's IContentLoader - hosts
/// do not need to call this directly. It remains public so hosts without EPiServer.Cms.Core
/// (e.g. the sample site) can register the same services against their own loader.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptiA11y(this IServiceCollection services)
    {
        services.AddSingleton<IContentRule, AltTextQualityRule>();
        services.AddSingleton<IContentRule, HeadingStructureRule>();
        services.AddSingleton<IContentRule, LinkPurposeRule>();
        services.AddSingleton<IContentRule, TableHeaderRule>();
        services.AddSingleton<IContentRule, MediaCaptionsRule>();
        services.AddSingleton<IContentRule, FormLabelRule>();
        services.AddSingleton<IContentRule, ButtonNameRule>();
        services.AddSingleton<IContentRule, IframeTitleRule>();
        services.AddSingleton<IContentRule, DeprecatedElementRule>();
        services.AddSingleton<IContentRule, InteractiveAttributesRule>();
        services.AddSingleton<IContentRule, FakeListRule>();
        services.AddSingleton<IContentRule, LanguageAttributeRule>();
        services.AddSingleton<IContentRule, ColorContrastRule>();
        services.AddSingleton<IContentRule, TextReadabilityRule>();
        services.AddSingleton<IContentRule, DocumentLinkExpectationsRule>();
        services.AddSingleton<IContentRule, LanguageOfPartsRule>();
        services.AddSingleton<IContentRule, SensoryCharacteristicsRule>();
        services.AddSingleton<IContentRule, ReadingLevelRule>();
        services.AddSingleton<IContentRule, LinkNameRule>();
        services.AddSingleton<IContentRule, FauxHeadingRule>();
        services.AddSingleton<IContentRule, SvgAccessibleNameRule>();
        services.AddSingleton<IContentRule, TableComplexityRule>();
        services.AddSingleton<IContentRule, AutoplayMediaRule>();
        services.AddSingleton<IContentRule, AudioDescriptionRule>();
        services.AddSingleton<IContentRule, InvalidAriaRule>();
        services.AddSingleton<IContentRule, BrokenAriaReferenceRule>();
        services.AddSingleton<IContentRule, NestedInteractiveRule>();
        services.AddSingleton<IContentRule, InputPurposeRule>();
        services.AddSingleton<IContentRule, FieldsetLegendRule>();
        services.AddSingleton<IContentRule, TitleAttributeMisuseRule>();
        services.AddSingleton<IContentRule, PageTitleRule>();
        services.AddSingleton<IContentRule, TargetSizeRule>();
        services.AddSingleton<IContentRule, FocusIndicatorRule>();
        services.AddSingleton<IContentRule, MotionRule>();
        services.AddSingleton<IContentRule, ReflowRule>();
        services.AddSingleton<IContentRule, TextSpacingRule>();
        services.AddSingleton<IContentRule, ImageOfTextRule>();
        services.AddSingleton<IContentRule, DecorativeImageMisuseRule>();
        services.AddSingleton<IContentRule, FigureCaptionMismatchRule>();
        services.AddSingleton<IContentRule, ComplexImageDescriptionRule>();
        services.AddSingleton<IContentRule, LayoutTableRule>();
        services.AddSingleton<IContentRule, ListMisuseRule>();
        services.AddSingleton<IContentRule, DefinitionListStructureRule>();
        services.AddSingleton<IContentRule, BlockquoteMisuseRule>();
        services.AddSingleton<IContentRule, DuplicateIdRule>();
        services.AddSingleton<IContentRule, HeadingLengthRule>();
        services.AddSingleton<IContentRule, LabelQualityRule>();
        services.AddSingleton<IContentRule, EmphasisMisuseRule>();
        services.AddSingleton<IContentRule, SuperscriptSubscriptMisuseRule>();
        services.AddSingleton<IContentRule, MeaningfulSequenceRule>();
        services.AddSingleton<IContentRule, SameDestinationDifferentTextRule>();
        services.AddSingleton<IContentRule, NewWindowLinkRule>();
        services.AddSingleton<IContentRule, AdjacentDuplicateLinksRule>();
        services.AddSingleton<IContentRule, SkipLinkTargetRule>();
        services.AddSingleton<IContentRule, MetaRefreshRule>();
        services.AddSingleton<IContentRule, PlaceholderAsLabelRule>();
        services.AddSingleton<IContentRule, RequiredFieldIndicationRule>();
        services.AddSingleton<IContentRule, ErrorIdentificationRule>();
        services.AddSingleton<IContentRule, FormInstructionsRule>();
        services.AddSingleton<IContentRule, SelectOptionQualityRule>();
        services.AddSingleton<IContentRule, ReadonlyDisabledMisuseRule>();
        services.AddSingleton<IContentRule, RedundantEntryRule>();
        services.AddSingleton<IContentRule, InputTypeAppropriatenessRule>();
        services.AddSingleton<IContentRule, AriaRequiredChildrenRule>();
        services.AddSingleton<IContentRule, AriaRequiredAttributesRule>();
        services.AddSingleton<IContentRule, AriaAllowedAttributeRule>();
        services.AddSingleton<IContentRule, RedundantRoleRule>();
        services.AddSingleton<IContentRule, AriaHiddenFocusableRule>();
        services.AddSingleton<IContentRule, PresentationRoleConflictRule>();
        services.AddSingleton<IContentRule, LandmarkStructureRule>();
        services.AddSingleton<IContentRule, LiveRegionMisuseRule>();
        services.AddSingleton<IContentRule, AbbreviationExpansionRule>();
        services.AddSingleton<IContentRule, UnusualWordsRule>();
        services.AddSingleton<IContentRule, PronunciationAmbiguityRule>();
        services.AddSingleton<IContentRule, UnicodeStyledTextRule>();
        services.AddSingleton<IContentRule, EmojiOveruseRule>();
        services.AddSingleton<IContentRule, AsciiArtRule>();
        services.AddSingleton<IContentRule, WhitespaceFormattingRule>();
        services.AddSingleton<IContentRule, LineBreakMisuseRule>();
        services.AddSingleton<IContentRule, LinkTextLanguageRule>();
        services.AddSingleton<IContentRule, BlinkingContentRule>();
        services.AddSingleton<IContentRule, TimedContentRule>();
        services.AddSingleton<IContentRule, MediaTranscriptQualityRule>();
        services.AddSingleton<IContentRule, SignLanguageRule>();
        services.AddSingleton<IContentRule, ExtendedAudioDescriptionRule>();
        services.AddSingleton<IContentRule, MediaAlternativeRule>();
        services.AddSingleton<IContentRule, FlashingContentRule>();

        services.AddSingleton<IContentRule, UseOfColorRule>();
        services.AddSingleton<IContentRule, LinkDistinguishabilityRule>();
        services.AddSingleton<IContentRule, NonTextContrastRule>();
        services.AddSingleton<IContentRule, ContrastEnhancedRule>();
        services.AddSingleton<IContentRule, LineLengthRule>();
        services.AddSingleton<IContentRule, LineSpacingRule>();
        services.AddSingleton<IContentRule, BackgroundImageTextRule>();

        services.AddSingleton<IContentRule, KeyboardTrapRule>();
        services.AddSingleton<IContentRule, FocusOrderRule>();
        services.AddSingleton<IContentRule, FocusNotObscuredRule>();
        services.AddSingleton<IContentRule, FocusAppearanceRule>();
        services.AddSingleton<IContentRule, KeyboardOperableRule>();
        services.AddSingleton<IContentRule, HoverFocusContentRule>();
        services.AddSingleton<IContentRule, OrientationLockRule>();
        services.AddSingleton<IContentRule, ResizeTextRule>();
        services.AddSingleton<IContentRule, StickyObstructionRule>();
        services.AddSingleton<IContentRule, FlashThresholdRule>();
        services.AddSingleton<IContentRule, PointerTargetSpacingRule>();
        services.AddSingleton<IContentRule, PointerGesturesRule>();
        services.AddSingleton<IContentRule, DraggingMovementsRule>();
        services.AddSingleton<IContentRule, LabelInNameRule>();
        services.AddSingleton<IContentRule, StatusMessagesRule>();
        services.AddSingleton<IContentRule, BypassBlocksRule>();
        services.AddSingleton<IContentRule, LandmarkCompletenessRule>();
        services.AddSingleton<IContentRule, HeadingInViewportOrderRule>();

        services.AddSingleton(sp => new RuleEngine(sp.GetServices<IContentRule>()));

        services.AddScoped<IContentAuditDocumentAdapter, PaasContentAdapter>();
        services.AddScoped<RunAuditHandler>();

        return services;
    }

    /// <summary>
    /// Opt-in registration for the rendered-style slice: enables <see cref="ColorContrastRule"/>
    /// and <see cref="TextReadabilityRule"/> to evaluate real computed CSS (from external
    /// stylesheets/classes/themes) rather than only inline <c>style</c> attributes, by rendering
    /// each content item in headless Chromium via Playwright.
    ///
    /// This registers a singleton <see cref="PlaywrightRenderedStyleProvider"/>, which requires
    /// Playwright's browser binaries to be installed on the host once
    /// (<c>playwright install chromium</c>); rendering fails soft and the audit falls back to
    /// inline-style-only fragments if the browser is unavailable.
    ///
    /// Hosts must also register their own <see cref="IContentPreviewUrlResolver"/> (replacing the
    /// default no-op) so a public/preview URL can be resolved per content item; without one,
    /// enrichment is skipped entirely.
    /// </summary>
    public static IServiceCollection AddOptiA11yRenderedStyles(this IServiceCollection services)
    {
        services.AddSingleton<IRenderedStyleProvider, PlaywrightRenderedStyleProvider>();
        services.TryAddSingleton<IContentPreviewUrlResolver, NullContentPreviewUrlResolver>();

        return services;
    }
}

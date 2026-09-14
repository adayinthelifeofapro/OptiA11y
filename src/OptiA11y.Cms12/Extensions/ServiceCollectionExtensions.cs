using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OptiA11y.Cms12.Features.RunAudit;
using OptiA11y.Cms12.Infrastructure.ContentAdapter;
using OptiA11y.Core.Rules;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.AudioDescription;
using OptiA11y.Core.Rules.AutoplayMedia;
using OptiA11y.Core.Rules.BrokenAriaReference;
using OptiA11y.Core.Rules.ButtonName;
using OptiA11y.Core.Rules.ColorContrast;
using OptiA11y.Core.Rules.DeprecatedElements;
using OptiA11y.Core.Rules.DocumentLinkExpectations;
using OptiA11y.Core.Rules.FauxHeading;
using OptiA11y.Core.Rules.FieldsetLegend;
using OptiA11y.Core.Rules.FocusIndicator;
using OptiA11y.Core.Rules.FormLabels;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.IframeTitle;
using OptiA11y.Core.Rules.InputPurpose;
using OptiA11y.Core.Rules.InteractiveAttributes;
using OptiA11y.Core.Rules.InvalidAria;
using OptiA11y.Core.Rules.LanguageAttribute;
using OptiA11y.Core.Rules.LanguageOfParts;
using OptiA11y.Core.Rules.LinkName;
using OptiA11y.Core.Rules.LinkPurpose;
using OptiA11y.Core.Rules.ListStructure;
using OptiA11y.Core.Rules.MediaCaptions;
using OptiA11y.Core.Rules.Motion;
using OptiA11y.Core.Rules.NestedInteractive;
using OptiA11y.Core.Rules.PageTitle;
using OptiA11y.Core.Rules.ReadingLevel;
using OptiA11y.Core.Rules.Reflow;
using OptiA11y.Core.Rules.SensoryCharacteristics;
using OptiA11y.Core.Rules.SvgAccessibleName;
using OptiA11y.Core.Rules.TableComplexity;
using OptiA11y.Core.Rules.TableHeaders;
using OptiA11y.Core.Rules.TargetSize;
using OptiA11y.Core.Rules.TextReadability;
using OptiA11y.Core.Rules.TextSpacing;
using OptiA11y.Core.Rules.TitleAttributeMisuse;
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

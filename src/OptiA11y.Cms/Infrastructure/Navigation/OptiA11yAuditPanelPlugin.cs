using EPiServer.Shell.ViewComposition;

namespace OptiA11y.Cms.Infrastructure.Navigation;

/// <summary>
/// Registers an accessibility audit panel in the CMS 13 edit view's right-hand assets pane.
/// Implemented as a <see cref="RelativeUrlIFrameComponentAttribute"/> plugin (a thin wrapper
/// around the same CMS <c>IFrameContextComponent</c> widget used by
/// <see cref="EPiServer.Shell.ViewComposition.IFrameComponentAttribute"/>, but without the
/// module-resource resolution that requires a registered shell module - see
/// <see cref="RelativeUrlIFrameComponentAttribute"/> for details) targeting the real, documented
/// plug-in area used by the built-in <c>MediaComponent</c>/<c>SharedBlocksComponent</c> plugins.
///
/// The CMS shell's context-aware iframe widget (<c>epi/shell/component/IFrameContextComponent</c>)
/// appends the currently-edited content's reference to <see cref="RelativeUrlIFrameComponentAttribute.Url"/>
/// as an <c>id</c> query string parameter and reloads the iframe (<see cref="RelativeUrlIFrameComponentAttribute.ReloadOnContextChange"/>
/// defaults to true) whenever the editor's content context changes, so this panel automatically
/// re-renders the audit for whatever page or block the editor currently has open.
/// </summary>
[RelativeUrlIFrameComponent(
    Url = OptiA11yAuditPanelPlugin.PanelUrl,
    PlugInAreas = "/episerver/cms/assets/defaultgroup",
    Categories = "content",
    Title = "Accessibility audit",
    SortOrder = 100,
    MinHeight = 400,
    MaxHeight = 1200)]
public sealed class OptiA11yAuditPanelPlugin
{
    public const string PanelUrl = "/optia11y/audit/panel";
}



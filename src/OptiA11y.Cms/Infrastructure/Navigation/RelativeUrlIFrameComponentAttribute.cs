using EPiServer.Shell.ViewComposition;

namespace OptiA11y.Cms.Infrastructure.Navigation;

/// <summary>
/// A minimal reimplementation of <see cref="IFrameComponentAttribute"/>'s behavior that treats
/// <see cref="Url"/> as a plain site-relative URL rather than a module-embedded resource path.
///
/// The stock <see cref="IFrameComponentAttribute"/> calls <c>EPiServer.Shell.Paths.ToResource</c>
/// for any non-absolute <see cref="Url"/>, which requires the attributed type's assembly to be a
/// registered CMS shell module with its own client-resource folder. OptiA11y.Cms is a plain
/// content/API add-on with no such module resources, so that call throws
/// <c>ArgumentException: Unable to find a module by assembly '...'</c>. This attribute uses the
/// same iframe widget (<c>epi/shell/component/IFrameContextComponent</c>) and settings shape, but
/// sets <c>urlTemplate</c> straight from <see cref="Url"/> so the CMS shell requests it as an
/// ordinary site-relative URL (still receiving the standard <c>contentLink</c>/<c>epi.env</c>
/// context query parameters and reload-on-context-change behavior).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RelativeUrlIFrameComponentAttribute : ComponentAttribute
{
    public override string WidgetType
    {
        get => "epi/shell/component/IFrameContextComponent";
        set { }
    }

    /// <summary>The site-relative source URL for the iframe to load.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Whether the iframe should be reloaded when the editor context changes. Defaults to true.</summary>
    public bool ReloadOnContextChange { get; set; } = true;

    /// <summary>Whether the iframe should keep its current URL when the context changes and it is reloaded. Defaults to false.</summary>
    public bool KeepUrlOnContextChange { get; set; }

    /// <summary>The minimum height of the iframe, in pixels. Defaults to 100.</summary>
    public int MinHeight { get; set; } = 100;

    /// <summary>The maximum height of the iframe, in pixels. Defaults to 500.</summary>
    public int MaxHeight { get; set; } = 500;

    public override IComponentDefinition CreateComponentDefinition(Type attributedType)
    {
        Settings["urlTemplate"] = Url;
        Settings["reloadOnContextChange"] = ReloadOnContextChange;
        Settings["keepUrlOnContextChange"] = KeepUrlOnContextChange;
        Settings["minHeight"] = MinHeight;
        Settings["maxHeight"] = MaxHeight;

        return base.CreateComponentDefinition(attributedType);
    }
}

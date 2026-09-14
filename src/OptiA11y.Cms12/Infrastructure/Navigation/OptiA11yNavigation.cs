using EPiServer.Shell.Navigation;

namespace OptiA11y.Cms12.Infrastructure.Navigation;

/// <summary>
/// Describes a single navigation entry OptiA11y wants exposed in the CMS shell menu, pointing
/// at the standalone RunAudit view. This is the extensibility-safe route for slice one: the
/// CMS 13 editor's in-context panel extension surface (React, replacing Dojo) is unverified,
/// so we do not attempt to plug into it here. A standalone menu entry works regardless of how
/// that spike resolves.
/// </summary>
/// <param name="Name">A stable identifier for the menu entry.</param>
/// <param name="Text">The label shown to editors.</param>
/// <param name="Url">The relative URL the entry navigates to, matching <see cref="Features.RunAudit.RunAuditEndpoint.RoutePattern"/>'s host page.</param>
/// <param name="SortIndex">Ordering hint among sibling menu entries.</param>
public sealed record OptiA11yMenuEntry(string Name, string Text, string Url, int SortIndex);

/// <summary>
/// Supplies the fixed set of OptiA11y navigation entries.
/// </summary>
public static class OptiA11yNavigation
{
    public static IReadOnlyList<OptiA11yMenuEntry> MenuEntries { get; } = new[]
    {
        new OptiA11yMenuEntry(
            Name: "optia11y-audit",
            Text: "Accessibility audit",
            Url: "/optia11y/audit",
            SortIndex: 100)
    };
}

/// <summary>
/// Registers OptiA11y's menu entries in the CMS 13 shell menu. The <see cref="MenuProviderAttribute"/>
/// makes this discoverable automatically once the add-on assembly is present in the host's bin
/// folder - no manual registration is required in the host solution.
/// </summary>
[MenuProvider]
public sealed class OptiA11yMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        foreach (var entry in OptiA11yNavigation.MenuEntries)
        {
            yield return new UrlMenuItem(entry.Text, MenuPaths.Global + "/cms/" + entry.Name, entry.Url)
            {
                SortIndex = entry.SortIndex
            };
        }
    }
}

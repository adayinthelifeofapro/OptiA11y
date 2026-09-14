using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Navigation.Internal;

namespace EPiServer.Shell.Navigation.Providers;

/// <summary>
///       Help menu provider that lists all help files defined within the <see cref="T:EPiServer.Shell.Modules.ShellModule" />s <see cref="T:EPiServer.Shell.Configuration.ShellModuleManifest" /> file.
///       It uses the <see cref="T:EPiServer.Shell.Modules.ShellModule" />.Name when creating the menu item.
///       </summary>
internal class ModuleHelpMenuProvider : IMenuProvider, IEPiProductMenuProvider
{
	private readonly ModuleTable _moduleManager;

	private LocalizationService LocalizationService { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.Providers.ModuleHelpMenuProvider" /> class.
	///       </summary>
	/// <param name="moduleManager">Module manager to use.</param>
	/// <param name="localizationService">The service used for localization.</param>
	public ModuleHelpMenuProvider(ModuleTable moduleManager, LocalizationService localizationService)
	{
		_moduleManager = moduleManager;
		LocalizationService = localizationService;
	}

	/// <summary>
	///       This method is called when the menu is being assembled.
	///       </summary>
	/// <returns>
	///       A list of <see cref="T:EPiServer.Shell.Navigation.MenuItem" />s that the provider exposes.
	///       </returns>
	/// <remarks>
	///       The Provider implementation has to handle the security themself.
	///       </remarks>
	public IEnumerable<MenuItem> GetMenuItems()
	{
		List<ShellModule> list = (from m in _moduleManager.GetModules()
			where !string.IsNullOrEmpty(m.Manifest.HelpFile)
			select m).ToList();
		List<MenuItem> list2 = new List<MenuItem>();
		string text = LocalizationService.GetString("/EPiServer/Shell/Resources/Texts/Help");
		list2.Add(new DropDownMenuItem(text, "/global/help")
		{
			Alignment = MenuItemAlignment.Right,
			CssClass = "epi-navigation-iconic",
			ToolTip = text,
			SortIndex = 970
		});
		foreach (ShellModule item in list)
		{
			string path = "/global/help/" + item.Name.ToLowerInvariant();
			string helpUrl = item.GetHelpUrl();
			string text2 = item.Manifest.ProductName;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = item.Name;
			}
			list2.Add(new UrlMenuItem(text2, path, helpUrl)
			{
				Target = "_blank"
			});
		}
		return list2;
	}
}

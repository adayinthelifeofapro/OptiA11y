using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Navigation.Providers;

/// <summary>
///       Provides menu items configured in web.config
///       </summary>
internal class ConfigNavigationProvider : IMenuProvider
{
	private readonly MenuItem[] _menuItems;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.Providers.ConfigNavigationProvider" /> class.
	///       </summary>
	/// <param name="options">The NavigationOptions.</param>
	public ConfigNavigationProvider(NavigationOptions options)
	{
		_menuItems = options.Items.Select(CreateMenuItem).ToArray();
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
		return _menuItems;
	}

	private MenuItem CreateMenuItem(NavigationDetails detail)
	{
		MenuItem menuItem = null;
		if (detail.MenuItemType != MenuItemType.Link)
		{
			menuItem = ((detail.MenuItemType != MenuItemType.DropDown) ? ((MenuItem)new SectionMenuItem(detail.Text, detail.MenuPath)
			{
				SortIndex = detail.SortIndex,
				ToolTip = detail.ToolTip
			}) : ((MenuItem)new DropDownMenuItem(detail.Text, detail.MenuPath)
			{
				SortIndex = detail.SortIndex,
				ToolTip = detail.ToolTip
			}));
		}
		else
		{
			menuItem = new UrlMenuItem(detail.Text, detail.MenuPath, detail.Url)
			{
				SortIndex = detail.SortIndex,
				ToolTip = detail.ToolTip
			};
			if (!string.IsNullOrEmpty(detail.Target))
			{
				menuItem.Target = detail.Target;
			}
		}
		menuItem.Alignment = detail.Alignment;
		return menuItem;
	}
}

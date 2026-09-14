using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Represents the details of the navigation inside episerver.shell.navigation configuration.
///       </summary>
public class NavigationDetails
{
	/// <summary>
	///       Menu item alignment
	///       </summary>
	public MenuItemAlignment Alignment { get; set; }

	/// <summary>
	///       The type of this menu item
	///       </summary>
	public MenuItemType MenuItemType { get; set; }

	/// <summary>
	///       The menu path/key of this menu item. The path defines the structure and is also the unique key to this menu item.
	///       </summary>
	public string MenuPath { get; set; }

	/// <summary>
	///       The sort index of this menu item. Menu items are sorted from low to high within each section.
	///       </summary>
	public int SortIndex { get; set; }

	/// <summary>
	///       The menu link target frame.
	///       </summary>
	public string Target { get; set; }

	/// <summary>
	///       The menu text
	///       </summary>
	public string Text { get; set; }

	/// <summary>
	///       The target url of this menu item.
	///       </summary>
	public string Url { get; set; }

	/// <summary>
	///       The tooltip of this menu item.
	///       </summary>
	public string ToolTip { get; set; }
}

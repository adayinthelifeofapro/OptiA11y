using System.Collections.Generic;
using EPiServer.Shell.Navigation;

namespace EPiServer.Shell.Web.Internal;

/// <summary>
///       Represents a single navigation item displayed in the platform navigation
///       </summary>
public class NavigationItem
{
	/// <summary>
	///       Name of the item
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       Url of the item
	///       </summary>
	public string Url { get; set; }

	/// <summary>
	///       Target frame
	///       </summary>
	/// <example>
	///       _blank
	///       </example>
	public string Target { get; set; }

	/// <summary>
	///       Children of the item
	///       </summary>
	public IEnumerable<NavigationItem> Children { get; set; }

	/// <summary>
	///       When true, then menu item should be rendered
	///       </summary>
	public bool IsAvailable { get; set; } = true;

	/// <summary>
	///       Name of Icon
	///       </summary>
	public string IconName { get; set; }

	/// <summary>
	///       Css class
	///       </summary>
	public string CssClass { get; set; }

	/// <summary>
	///       Path to menu item used to build hierarchy
	///       </summary>
	public string Path { get; set; }

	/// <summary>
	///       Get or sets the flag to indicate the menu item is EPi Product or not.
	///       assigned from <see cref="T:EPiServer.Shell.Navigation.MenuItem" /></summary>
	public bool IsEPiMenuItem { get; set; }

	/// <summary>
	///       The behavior of the menu item.
	///       </summary>
	public MenuItemBehavior Behavior { get; set; }

	/// <summary>
	///       The tooltip of the item
	///       </summary>
	public string Tooltip { get; set; }
}

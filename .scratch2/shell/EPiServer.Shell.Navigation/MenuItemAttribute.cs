using System;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Declares a menu item.
///       </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MenuItemAttribute : MenuAttributeBase
{
	/// <summary>
	///       Gets or sets the URL corresponding o the menu item.
	///       </summary>
	public string Url { get; set; }

	/// <summary>
	///       The CSS class for the menu item
	///       </summary>
	public string CssClass { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuItemAttribute" /> class.
	///       </summary>
	/// <param name="menuPath">The unique path of the menu item.</param>
	public MenuItemAttribute(string menuPath)
		: base(menuPath)
	{
	}

	/// <summary>
	///       Resolves and returns the <see cref="P:EPiServer.Shell.Navigation.MenuItemAttribute.Url" /> parameter.
	///       </summary>
	/// <returns>The resolved <see cref="P:EPiServer.Shell.Navigation.MenuItemAttribute.Url" /> parameter.</returns>
	public virtual string GetResolvedUrl()
	{
		return Url;
	}
}

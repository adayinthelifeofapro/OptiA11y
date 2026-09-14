using System;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Defines a menu section when placed on a <see cref="T:EPiServer.Shell.Web.Mvc.IController" /></summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MenuSectionAttribute : MenuAttributeBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuSectionAttribute" /> class.
	///       </summary>
	/// <param name="menuPath">The unique path of the menu item.</param>
	public MenuSectionAttribute(string menuPath)
		: base(menuPath)
	{
	}
}

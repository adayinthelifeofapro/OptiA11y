namespace EPiServer.Shell.Navigation;

/// <summary>
///       A top menu item that opens other menu items in a drop down menu.
///       </summary>
public class DropDownMenuItem : MenuItem
{
	/// <summary>
	///       Gets the css to apply on the li tag that encapsulates the link
	///       </summary>
	/// <value>
	/// </value>
	protected internal override string NodeCssClass => base.NodeCssClass + " epi-navigation-dropdown";

	/// <summary>
	///       Drop downs are not leaves.
	///       </summary>
	/// <value>
	/// </value>
	public override bool IsLeaf => false;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.DropDownMenuItem" /> class.
	///       </summary>
	/// <param name="text">Link text</param>
	/// <param name="path">Unique path for the menu item</param>
	public DropDownMenuItem(string text, string path)
		: base(text, path)
	{
		base.Url = "#" + this.GetSubmenuClientId();
		base.IsStyled = true;
	}
}

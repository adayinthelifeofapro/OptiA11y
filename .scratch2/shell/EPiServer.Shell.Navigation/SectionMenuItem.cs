namespace EPiServer.Shell.Navigation;

/// <summary>
///       A top menu item that contains other menu items as children
///       </summary>
public class SectionMenuItem : MenuItem
{
	/// <summary>
	///       Gets the css to apply on the li tag that encapsulates the link
	///       </summary>
	/// <value>
	/// </value>
	protected internal override string NodeCssClass => "epi-navigation-standard " + base.NodeCssClass;

	/// <summary>
	///       Menu sections are not leaves.
	///       </summary>
	/// <value>
	/// </value>
	public override bool IsLeaf => false;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.SectionMenuItem" /> class.
	///       </summary>
	/// <param name="text">Link text</param>
	/// <param name="path">Unique path for the menu item</param>
	public SectionMenuItem(string text, string path)
		: base(text, path)
	{
		base.Url = "#" + this.GetSubmenuClientId();
	}
}

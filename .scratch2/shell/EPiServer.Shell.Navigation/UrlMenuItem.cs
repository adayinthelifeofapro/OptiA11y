namespace EPiServer.Shell.Navigation;

/// <summary>
///       Basic URI menu item
///       </summary>
public class UrlMenuItem : MenuItem
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.UrlMenuItem" /> class.
	///       </summary>
	/// <param name="text">Link text</param>
	/// <param name="path">Unique path for the menu item</param>
	/// <param name="url">Link Url</param>
	public UrlMenuItem(string text, string path, string url)
		: base(text, path)
	{
		base.Url = url;
	}
}

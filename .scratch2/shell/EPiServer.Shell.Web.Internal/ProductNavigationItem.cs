namespace EPiServer.Shell.Web.Internal;

/// <summary>
///       Represents products in global navigation
///       </summary>
public class ProductNavigationItem : NavigationItem
{
	/// <summary>
	///       Gets or sets the id of the product. It will be the client id of the menu item.
	///       </summary>
	public string Id { get; set; }
}

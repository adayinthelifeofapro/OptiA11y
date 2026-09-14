using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Web.Internal;

/// <summary>
///       Represents global user interface settings
///       </summary>
[Options]
public class UserInterfaceOptions
{
	/// <summary>
	///       Gets or sets the the theme class(es) used for the body element.
	///       </summary>
	public string ThemeClass { get; set; } = "Sleek";

	/// <summary>
	///       Maximum number of items in Assets ContentFolder when children will be paged
	///       </summary>
	public int MaxContentFolderChildrenWithoutPaging { get; set; } = 1000;

	/// <summary>
	///       Maximum number of child items under a single node in Content Tree
	///       </summary>
	public int MaxChildrenCountInContentTree { get; set; } = 3000;
}

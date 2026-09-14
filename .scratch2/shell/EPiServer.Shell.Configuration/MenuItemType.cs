namespace EPiServer.Shell.Configuration;

/// <summary>
///       What type of item the configuration represents
///       </summary>
public enum MenuItemType
{
	/// <summary>
	///       A link to a web resource
	///       </summary>
	Link,
	/// <summary>
	///       A menu section with subsections with a path below this section
	///       </summary>
	Section,
	/// <summary>
	///       A drop down style menu
	///       </summary>
	DropDown
}

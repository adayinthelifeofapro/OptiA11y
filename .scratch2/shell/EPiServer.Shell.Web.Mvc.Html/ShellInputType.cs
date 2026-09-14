namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Used to define what input type to use.
///       </summary>
/// <remarks>
///       Used by the ShellInput and ShellButton helper method.
///       </remarks>
public enum ShellInputType
{
	/// <summary>
	///       Standard button input
	///       </summary>
	Button,
	/// <summary>
	///       Checkbox type input
	///       </summary>
	CheckBox,
	/// <summary>
	///       file type input
	///       </summary>
	File,
	/// <summary>
	///       hidden type input
	///       </summary>
	Hidden,
	/// <summary>
	///        image type input
	///       </summary>
	Image,
	/// <summary>
	///       password type input
	///       </summary>
	Password,
	/// <summary>
	///       radiobutton type input
	///       </summary>
	Radio,
	/// <summary>
	///       reset button input
	///       </summary>
	Reset,
	/// <summary>
	///       Submit type button
	///       </summary>
	Submit,
	/// <summary>
	///       text type input
	///       </summary>
	Text
}

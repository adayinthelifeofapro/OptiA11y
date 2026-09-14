namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Used by the <see cref="T:EPiServer.Shell.Web.Mvc.Html.Pager" /> to handle UI pageing. This class is responsible for handling the position of the pager.
///       </summary>
public class PagerPosition
{
	/// <summary>
	///       The index for this page.
	///       </summary>
	public int Index { get; set; }

	/// <summary>
	///       The name that will be displayed when rendered.
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       Indicates if this is the currently active page.
	///       </summary>
	public bool IsCurrent { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.Html.PagerPosition" /> class.
	///       </summary>
	/// <param name="index">The index for this pag.</param>
	/// <param name="name">The name that will be displayed when rendered.</param>
	/// <param name="isCurrent">Indicates if this is the currently active page</param>
	public PagerPosition(int index, string name, bool isCurrent)
	{
		Index = index;
		Name = name;
		IsCurrent = isCurrent;
	}
}

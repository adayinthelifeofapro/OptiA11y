using System.Collections.Generic;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       A generic pageable list used as a view data model.
///       </summary>
/// <typeparam name="T">The type of objects that will be contained in the Items collection</typeparam>
public class Pageable<T>
{
	/// <summary>
	///       A list of items.
	///       </summary>
	public IEnumerable<T> Items { get; set; }

	/// <summary>
	///       The <see cref="T:EPiServer.Shell.Web.Mvc.Html.Pager" /> responsible for handling the paging.
	///       </summary>
	public Pager Pages { get; set; }
}

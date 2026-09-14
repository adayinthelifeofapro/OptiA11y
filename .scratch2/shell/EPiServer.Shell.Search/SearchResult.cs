using System.Collections.Generic;
using EPiServer.Shell.Web.Mvc.Html;

namespace EPiServer.Shell.Search;

/// <summary>
///       The model that is returned from from an <see cref="T:EPiServer.Shell.Search.ISearchProvider" />.
///       </summary>
public class SearchResult
{
	/// <summary>
	///       The URL to this search result.
	///       </summary>
	public string Url { get; set; }

	/// <summary>
	///       The title of this search result.
	///       </summary>
	public string Title { get; set; }

	/// <summary>
	///       A css class used when rendering this search result.
	///       </summary>
	public string IconCssClass { get; set; }

	/// <summary>
	///       A preview text for this search result.
	///       </summary>
	public string PreviewText { get; set; }

	/// <summary>
	///       The language the this search result is in.
	///       </summary>
	public string Language { get; set; }

	/// <summary>
	///       A list of tooltip elements
	///       </summary>
	public IList<ToolTipElement> ToolTipElements { get; private set; }

	/// <summary>
	///       Custom Meta Data not rendered
	///       </summary>
	public IDictionary<string, string> Metadata { get; private set; }

	/// <summary>
	///       The breadcrumbs of the result.
	///       </summary>
	public IEnumerable<BreadcrumbElement> Breadcrumbs { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchResult" /> class.
	///       </summary>
	/// <param name="url">The URL to open when this result is selected.</param>
	/// <param name="title">The title of this search result.</param>
	public SearchResult(string url, string title)
		: this(url, title, string.Empty)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchResult" /> class.
	///       </summary>
	/// <param name="url">The URL to open when this result is selected.</param>
	/// <param name="title">The title of this search result.</param>
	/// <param name="previewText">A preview text for this search result.</param>
	public SearchResult(string url, string title, string previewText)
	{
		ToolTipElements = new List<ToolTipElement>();
		Metadata = new Dictionary<string, string>();
		IconCssClass = string.Empty;
		Url = url;
		Title = title;
		PreviewText = previewText;
		Language = string.Empty;
	}
}

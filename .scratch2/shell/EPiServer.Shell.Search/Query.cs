using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Search;

/// <summary>
///       Query object passed to the <see cref="T:EPiServer.Shell.Search.ISearchProvider" /></summary>
public class Query
{
	/// <summary>
	///       Gets/Sets the maximum number of hits that should be returned
	///       </summary>
	public int MaxResults { get; set; }

	/// <summary>
	///       Gets/Sets the search query
	///       </summary>
	public string SearchQuery { get; set; }

	/// <summary>
	///       Gets or sets the roots used when searching.
	///       </summary>
	public IEnumerable<string> SearchRoots { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether to filter on culture.
	///       </summary>
	public bool FilterOnCulture { get; set; }

	/// <summary>
	///       Gets a collection of query parameters to be used by the search provider.
	///       </summary>
	public IDictionary<string, object> Parameters { get; private set; }

	/// <summary>
	///       Initializes a new instance of <see cref="T:EPiServer.Shell.Search.Query" /></summary>
	/// <param name="searchQuery">Search query</param>
	public Query(string searchQuery)
		: this(searchQuery, 10)
	{
	}

	/// <summary>
	///       Initializes a new instance of <see cref="T:EPiServer.Shell.Search.Query" /></summary>
	/// <param name="searchQuery">Search query</param>
	/// <param name="maxResults">Maximum number of hits that should be returned</param>
	public Query(string searchQuery, int maxResults)
		: this(searchQuery, maxResults, new Dictionary<string, object>())
	{
	}

	/// <summary>
	///       Initializes a new instance of <see cref="T:EPiServer.Shell.Search.Query" /></summary>
	/// <param name="searchQuery">Search query</param>
	/// <param name="parameters">A collection of query parameters</param>
	public Query(string searchQuery, IDictionary<string, object> parameters)
		: this(searchQuery, 10, parameters)
	{
	}

	/// <summary>
	///       Initializes a new instance of <see cref="T:EPiServer.Shell.Search.Query" /></summary>
	/// <param name="searchQuery">Search query</param>
	/// <param name="maxResults">Maximum number of hits that should be returned</param>
	/// <param name="parameters">A collection of query parameters</param>
	public Query(string searchQuery, int maxResults, IDictionary<string, object> parameters)
	{
		MaxResults = maxResults;
		SearchQuery = searchQuery;
		Parameters = parameters;
		SearchRoots = Enumerable.Empty<string>();
	}
}

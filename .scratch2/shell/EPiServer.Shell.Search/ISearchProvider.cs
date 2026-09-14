using System.Collections.Generic;

namespace EPiServer.Shell.Search;

/// <summary>
///       Definition of a search provider
///       </summary>
public interface ISearchProvider
{
	/// <summary>
	///       Specifies a unique key for the search provider.
	///       </summary>
	/// <remarks>Default implementation is the .net typename encoded so that '.' is replaced with '_'</remarks>
	string Key => GetType().FullName.Replace('.', '_');

	/// <summary>
	///       Area that the provider mapps to, used for spotlight searching
	///       </summary>
	string Area { get; }

	/// <summary>
	///       The category that the provider returns hits in
	///       </summary>
	string Category { get; }

	/// <summary>
	///       Executes a Search on the provider
	///       </summary>
	/// <param name="query">The query to execute</param>
	/// <returns>A list of search results</returns>
	IEnumerable<SearchResult> Search(Query query);
}

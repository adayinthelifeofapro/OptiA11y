namespace EPiServer.Shell.Rest;

/// <summary>
///       Represents a query interface for a specific type againts the rest store
///       </summary>
public interface IRestQuery<T>
{
	/// <summary>
	///       Gets the name of the query.
	///       </summary>
	/// <value>The name.</value>
	/// <remarks>This will be used to identify which query to call.</remarks>
	string Name { get; }

	/// <summary>
	///       Gets the rank (priority) of the Query.
	///       </summary>
	/// <remarks>This will be used to select which query to call in case several queries
	///       share the same <see cref="P:EPiServer.Shell.Rest.IRestQuery`1.Name" />. A query with a higher rank is tried before
	///       one with a lower rank. This can be used to inject a different query
	///       which is used under certain circumstances (if <see cref="M:EPiServer.Shell.Rest.IRestQuery`1.CanHandleQuery(EPiServer.Shell.Rest.IQueryParameters)" />
	///       returns true.</remarks>
	int Rank { get; }

	/// <summary>
	///       Decides whether the implementation is capable to handle the current query
	///       parameters, for example based on the reference id or the content provider.
	///       </summary>
	/// <param name="parameters">The query parameters.</param>
	/// <returns>True to handle the query, false to let the next query (by sort
	///       order) decide if it wants to handle the query.</returns>
	bool CanHandleQuery(IQueryParameters parameters);

	/// <summary>
	///       Apply your own getting, filtering, sorting and ranking (for example if you want to do sorting and paging
	///       in a database call to avoid querying for items that end up not being returned.
	///       </summary>
	/// <param name="parameters">The query parameters.</param>
	QueryRange<T> ExecuteQuery(IQueryParameters parameters);
}

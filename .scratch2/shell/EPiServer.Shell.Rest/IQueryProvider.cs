namespace EPiServer.Shell.Rest;

/// <summary>
///       Helps finding a IRestQuery which can execute the query.
///       </summary>
public interface IQueryProvider<T>
{
	/// <summary>
	///       Gets the query with the requested name with the
	///       highest rank that can handle the query parameters
	///       (as reported by the CanHandleQuery method).
	///       </summary>
	/// <param name="queryName">The query name.</param>
	/// <param name="parameters">The query parameters.</param>
	/// <returns>A matching query, or null if none can be matched.</returns>
	IRestQuery<T> GetQuery(string queryName, IQueryParameters parameters);
}

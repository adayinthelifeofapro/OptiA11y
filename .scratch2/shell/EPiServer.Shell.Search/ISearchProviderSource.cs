using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Search;

/// <summary>
///       Signature for component that is a source for <see cref="T:System.IServiceProvider" /> instances.
///       </summary>
public interface ISearchProviderSource
{
	/// <summary>
	///       Lists all <see cref="T:System.IServiceProvider"> instances for the source</see></summary>
	/// <returns>
	/// </returns>
	IEnumerable<ISearchProvider> List();

	/// <summary>
	///       Defines a change token that can be used to signal changes in the source.
	///       </summary>
	IChangeToken GetChangeToken()
	{
		return NullChangeToken.Singleton;
	}
}

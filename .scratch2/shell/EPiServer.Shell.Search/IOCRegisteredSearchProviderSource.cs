using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Search;

internal class IOCRegisteredSearchProviderSource : ISearchProviderSource
{
	private readonly IEnumerable<ISearchProvider> _searchProviders;

	public IOCRegisteredSearchProviderSource(IEnumerable<ISearchProvider> searchProviders)
	{
		IEnumerable<ISearchProvider> source = searchProviders.Where((ISearchProvider sp) => sp.Key.Contains('.'));
		if (source.Any())
		{
			throw new ArgumentException("SearchProviders " + string.Join(',', source.Select((ISearchProvider sp) => sp.Key)) + " has '.' character in Key which is not allowed");
		}
		_searchProviders = searchProviders;
	}

	public IEnumerable<ISearchProvider> List()
	{
		return _searchProviders;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Search;

/// <summary>
///       Search providers manager class.
///       </summary>
internal class SearchProviderManager : ISearchProviderManager
{
	private readonly ILogger<SearchProviderManager> _log;

	private readonly IEnumerable<ISearchProviderSource> _searchProviderSources;

	private readonly DynamicDataStoreFactory _storeFactory;

	private IEnumerable<ISearchProvider> _searchProviders;

	private CompositeChangeToken _changeToken;

	/// <summary>
	///       The dynamic data store used in search provider manager.
	///       </summary>
	public DynamicDataStore Store => _storeFactory.GetStore(typeof(SearchProviderSetting)) ?? _storeFactory.CreateStore(typeof(SearchProviderSetting));

	/// <summary>
	///       Initalizes a new instance of <see cref="T:EPiServer.Shell.Search.SearchProviderManager" />.
	///       </summary>
	/// <param name="searchProviderSources">All available providers</param>
	/// <param name="storeFactory">A repository used for the actual data reading and manipulation.</param>
	/// <param name="logger">The logger.</param>
	public SearchProviderManager(IEnumerable<ISearchProviderSource> searchProviderSources, DynamicDataStoreFactory storeFactory, ILogger<SearchProviderManager> logger)
	{
		_searchProviderSources = searchProviderSources;
		_searchProviders = searchProviderSources.SelectMany((ISearchProviderSource s) => s.List());
		_storeFactory = storeFactory;
		_log = logger;
		_changeToken = new CompositeChangeToken(searchProviderSources.Select((ISearchProviderSource p) => p.GetChangeToken()).ToArray());
		_changeToken.RegisterChangeCallback(ProviderChanged, null);
	}

	/// <summary>
	///       Use this method to get the available providers prioritizing those in the specified search area.
	///       </summary>
	/// <param name="searchArea">Area to search in</param>
	/// <param name="filterOnArea">
	/// </param>
	/// <returns>A list of search providers.</returns>
	public IList<ISearchProvider> ListProviders(string searchArea, bool filterOnArea)
	{
		if (_log.IsEnabled(LogLevel.Debug))
		{
			_log.ListingSearchProviders(searchArea);
		}
		List<SearchProviderPair> source = (from pp in GetProviderPairs()
			where pp.Setting.IsEnabled
			select pp).ToList();
		if (filterOnArea)
		{
			List<SearchProviderPair> list = source.Where((SearchProviderPair pp) => AreaMatchesSearch(pp.Provider.Area, searchArea)).ToList();
			if (list.Count == 0)
			{
				list = source.Where((SearchProviderPair pp) => pp.Provider.Area.StartsWith(searchArea, StringComparison.OrdinalIgnoreCase)).ToList();
			}
			source = list;
		}
		return (from pp in source
			orderby (!AreaMatchesSearch(pp.Provider.Area, searchArea)) ? 1 : 0, pp.Setting.SortIndex
			select pp.Provider).ToList();
	}

	/// <summary>
	///       Tries to the get provider with the provided key (default provider's type.FullName).
	///       </summary>
	/// <param name="providerKey">The provider key.</param>
	/// <param name="provider">The provider.</param>
	/// <returns>
	/// </returns>
	public bool TryGetProvider(string providerKey, out ISearchProvider provider)
	{
		provider = _searchProviders.FirstOrDefault((ISearchProvider p) => p.Key == providerKey);
		return provider != null;
	}

	/// <summary>
	///       Saves the changes to the Database in the given order
	///       </summary>
	public void SaveProviderSettings(IEnumerable<SearchProviderSetting> settings)
	{
		ResetProviderSettings();
		int num = 0;
		foreach (SearchProviderSetting setting in settings)
		{
			setting.SortIndex = num++;
			Store.Save((object)setting);
		}
	}

	/// <summary>
	///       Reset to default
	///       </summary>
	internal void ResetProviderSettings()
	{
		Store.DeleteAll();
	}

	/// <summary>
	///       Loads the settings from the database or create default values
	///       </summary>
	/// <returns>
	/// </returns>
	private IEnumerable<SearchProviderSetting> GetSettings()
	{
		_log.LoadingSearchSettings();
		return from sps in Store.LoadAll<SearchProviderSetting>()
			where sps != null
			orderby sps.SortIndex
			select sps;
	}

	/// <summary>
	///       Gets the provider pairs.
	///       </summary>
	/// <returns>
	/// </returns>
	public IEnumerable<SearchProviderPair> GetProviderPairs()
	{
		List<SearchProviderPair> pairs = new List<SearchProviderPair>();
		foreach (SearchProviderSetting setting in GetSettings())
		{
			foreach (ISearchProvider item in (!string.IsNullOrEmpty(setting.ProviderKey)) ? _searchProviders.Where((ISearchProvider p) => p.Key.Equals(setting.ProviderKey, StringComparison.OrdinalIgnoreCase)) : _searchProviders.Where((ISearchProvider p) => p.GetType().FullName.Equals(setting.FullName)))
			{
				pairs.Add(new SearchProviderPair(setting, item));
			}
		}
		foreach (ISearchProvider item2 in _searchProviders.Where((ISearchProvider sp) => !pairs.Select((SearchProviderPair p) => p.Provider).Contains(sp)))
		{
			ISortable val = (ISortable)((item2 is ISortable) ? item2 : null);
			int sortIndex = ((val != null) ? val.SortOrder : 1000);
			pairs.Add(new SearchProviderPair(new SearchProviderSetting(item2.Key, item2.GetType().FullName, sortIndex, isEnabled: true), item2));
		}
		return pairs.Except(GetProvidersToSkip(pairs));
	}

	/// <summary>
	///       Gets a list of providers which should not be used.
	///       If for example a custom WCF search service or a FIND search provider
	///       then we should no longer use the built-in basic implementation.
	///       </summary>
	private IEnumerable<SearchProviderPair> GetProvidersToSkip(IEnumerable<SearchProviderPair> providers)
	{
		foreach (IGrouping<string, SearchProviderPair> item in from o in providers
			group o by o.Provider.Area)
		{
			if (item.Count() == 1)
			{
				continue;
			}
			foreach (SearchProviderPair item2 in item)
			{
				ISearchProvider provider = item2.Provider;
				ISortable val = (ISortable)((provider is ISortable) ? provider : null);
				if (val != null && val.SortOrder == int.MinValue)
				{
					yield return item2;
				}
			}
		}
	}

	/// <summary>
	///       Checks if a provider area matches the search area using boundary-aware matching.
	///       Matches exact area or sub-areas separated by '/', but not partial matches like
	///       "CMS/pages" matching "CMS/pages-graph".
	///       </summary>
	private static bool AreaMatchesSearch(string providerArea, string searchArea)
	{
		if (!providerArea.Equals(searchArea, StringComparison.OrdinalIgnoreCase))
		{
			return providerArea.StartsWith(searchArea + "/", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private void ProviderChanged(object obj)
	{
		_searchProviders = _searchProviderSources.SelectMany((ISearchProviderSource s) => s.List());
		_changeToken = new CompositeChangeToken(_searchProviderSources.Select((ISearchProviderSource p) => p.GetChangeToken()).ToArray());
		_changeToken.RegisterChangeCallback(ProviderChanged, null);
	}
}

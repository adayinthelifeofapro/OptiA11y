using System.Collections.Generic;

namespace EPiServer.Shell.Search;

/// <summary>
///       Manages search providers
///       </summary>
public interface ISearchProviderManager
{
	/// <summary>
	///       Gets the enabled search providers by priority.
	///       </summary>
	/// <param name="searchArea">The search area to filter by.</param>
	/// <param name="filterOnArea">Whether to filter strictly by area.</param>
	/// <returns>A list of enabled search providers.</returns>
	IList<ISearchProvider> ListProviders(string searchArea, bool filterOnArea);

	/// <summary>
	///       Tries to get a provider by its key.
	///       </summary>
	/// <param name="providerKey">The key of the provider.</param>
	/// <param name="provider">The found provider, if any.</param>
	/// <returns>True if found, otherwise false.</returns>
	bool TryGetProvider(string providerKey, out ISearchProvider provider);

	/// <summary>
	///       Saves the given search provider settings.
	///       </summary>
	/// <param name="settings">The settings to save.</param>
	void SaveProviderSettings(IEnumerable<SearchProviderSetting> settings);

	/// <summary>
	///       Gets all provider-setting pairs.
	///       </summary>
	/// <returns>A collection of provider-setting pairs.</returns>
	IEnumerable<SearchProviderPair> GetProviderPairs();
}

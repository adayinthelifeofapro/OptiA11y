using EPiServer.Data.Dynamic;

namespace EPiServer.Shell.Search;

/// <summary>
///       Object stored in the database with information about a search provider.
///       </summary>
[EPiServerDataStore(AutomaticallyRemapStore = true, SeamlessUpgrade = true)]
public class SearchProviderSetting
{
	/// <summary>
	///       Gets or sets the key for the provider
	///       </summary>
	public string ProviderKey { get; set; }

	/// <summary>
	///       Gets or sets the full type name of the search provider.
	///       </summary>
	public string FullName { get; set; }

	/// <summary>
	///       Gets or sets the sort index of this search provider.
	///       </summary>
	public int SortIndex { get; set; }

	/// <summary>
	///       Gets or sets whether the search provider is enabled.
	///       </summary>
	public bool IsEnabled { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchProviderSetting" /> class.
	///       </summary>
	public SearchProviderSetting()
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchProviderSetting" /> class.
	///       </summary>
	/// <param name="providerKey">The key of the provider.</param>
	/// <param name="providerTypeName">Name of the provider type.</param>
	/// <param name="sortIndex">Index of the sort.</param>
	/// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
	public SearchProviderSetting(string providerKey, string providerTypeName, int sortIndex, bool isEnabled)
	{
		ProviderKey = providerKey;
		FullName = providerTypeName;
		SortIndex = sortIndex;
		IsEnabled = isEnabled;
	}
}

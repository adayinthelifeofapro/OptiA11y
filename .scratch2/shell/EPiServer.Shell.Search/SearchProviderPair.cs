namespace EPiServer.Shell.Search;

/// <summary>
///       Used to pass configurable information about a search provider.
///       </summary>
public class SearchProviderPair
{
	/// <summary>
	///       Gets the provider typename encoded for usage on the client.
	///       </summary>
	public string ProviderKey => Provider.Key;

	/// <summary>
	///       Gets or sets the setting object stored in database or default.
	///       </summary>
	public SearchProviderSetting Setting { get; set; }

	/// <summary>
	///       Gets or sets the search provider.
	///       </summary>
	public ISearchProvider Provider { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchProviderPair" /> class.
	///       </summary>
	/// <param name="setting">The setting.</param>
	/// <param name="provider">The provider.</param>
	public SearchProviderPair(SearchProviderSetting setting, ISearchProvider provider)
	{
		Setting = setting;
		Provider = provider;
	}
}

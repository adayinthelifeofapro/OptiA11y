using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Search;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Shell.Navigation.Providers;

/// <summary>
///       Provides search for the shell menu items.
///       </summary>
internal class NavigationSearchProvider : ISearchProvider
{
	private readonly IEnumerable<IMenuProvider> _providers;

	private readonly IHttpContextAccessor _contextAccessor;

	private readonly LocalizationService _localizationService;

	/// <summary>
	///       Area that the provider mapps to, used for spotlight searching
	///       </summary>
	/// <value>Shell</value>
	public string Area => "Shell";

	/// <summary>
	///       The category that the provider returns hits in
	///       </summary>
	public string Category => _localizationService.GetString("/EPiServer/Shell/Resources/Texts/MenuSearchProviderCategory");

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.Providers.NavigationSearchProvider" /> class.
	///       </summary>
	/// <param name="providers">Available menu providers.</param>
	/// <param name="contextAccessor">Used to retrieve the request context at runtime.</param>
	/// <param name="localizationService">The localizationService used for translation.</param>
	public NavigationSearchProvider(IEnumerable<IMenuProvider> providers, IHttpContextAccessor contextAccessor, LocalizationService localizationService)
	{
		_providers = providers;
		_contextAccessor = contextAccessor;
		_localizationService = localizationService;
	}

	/// <summary>
	///       Gives an enumeration of menu items matching the query.
	///       </summary>
	/// <param name="query">Text in the menu items to look for.</param>
	/// <returns>An enumeration of menu items.</returns>
	public IEnumerable<SearchResult> Search(Query query)
	{
		HttpContext request = _contextAccessor.HttpContext;
		if (request != null)
		{
			string searchQuery = query.SearchQuery?.ToLower(CultureInfo.CurrentCulture);
			if (string.IsNullOrWhiteSpace(searchQuery))
			{
				return Enumerable.Empty<SearchResult>();
			}
			return (from m in (from m in _providers.SelectMany((IMenuProvider p) => p.GetMenuItems())
					where m.IsLeaf && Matches(m, searchQuery) && m.IsAvailable(request) && !m.ExcludeFromSearch
					orderby m.SortIndex, m.Text
					select m).Take(query.MaxResults)
				select new SearchResult(m.Url, m.Text, m.ToolTip)).ToList();
		}
		return Enumerable.Empty<SearchResult>();
	}

	private static bool Matches(MenuItem menuItem, string query)
	{
		if (menuItem.Text != null && menuItem.Text.ToLower(CultureInfo.CurrentCulture).Contains(query))
		{
			return true;
		}
		if (menuItem.ToolTip != null && menuItem.ToolTip.ToLower(CultureInfo.CurrentCulture).Contains(query))
		{
			return true;
		}
		return false;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using EPiServer.Shell.Navigation;
using EPiServer.Shell.Navigation.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Shell.Web.Internal;

/// <summary>
///       Service used to retrieve the page structure
///       </summary>
public class NavigationService
{
	private readonly MenuAssembler _menuAssembler;

	private readonly IHttpContextAccessor _requestContext;

	private readonly IAuthorizationService _authorizationService;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Internal.NavigationService" /> class.
	///       </summary>
	public NavigationService(MenuAssembler menuAssembler, IHttpContextAccessor requestContext, IAuthorizationService authorizationService)
	{
		_menuAssembler = menuAssembler;
		_requestContext = requestContext;
		_authorizationService = authorizationService;
	}

	/// <summary>
	///       Gets the current product.
	///       </summary>
	private async Task<MenuNode> GetCurrentProductAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return await _menuAssembler.GetMenuHierarchyAsync(string.Empty, 4, cancellationToken).FirstOrDefaultAsync((MenuNode x) => x.IsSelected, cancellationToken);
	}

	/// <summary>
	///       Gets the current product id.
	///       </summary>
	public async Task<string> GetCurrentProductIdAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return (await GetCurrentProductAsync(cancellationToken))?.Current.GetClientId();
	}

	/// <summary>
	///       Gets menu items for the specified product id.
	///       </summary>
	public async IAsyncEnumerable<NavigationItem> GetProductMenuItemsAsync(string productId, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		await foreach (MenuNode item in _menuAssembler.GetMenuHierarchyAsync("/global", 4, cancellationToken))
		{
			if (!string.Equals(item.Current.GetClientId(), productId, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			foreach (MenuNode menu in item.Children)
			{
				if (await IsAvailableAsync(menu))
				{
					yield return await ToNavigationItemAsync(menu);
				}
			}
		}
	}

	/// <summary>
	///       Gets all products.
	///       </summary>
	/// <returns>
	/// </returns>
	public async IAsyncEnumerable<ProductNavigationItem> GetAllProductsAsync()
	{
		await foreach (MenuNode menu in _menuAssembler.GetMenuHierarchyAsync("/global", 4))
		{
			bool flag = IsMainNavigation(menu);
			if (flag)
			{
				flag = await IsAvailableAsync(menu);
			}
			if (flag)
			{
				yield return await ToProductNavigationItemAsync(menu);
			}
		}
	}

	/// <summary>
	///       Gets all action items and their menu items
	///       </summary>
	/// <returns>
	/// </returns>
	public async IAsyncEnumerable<NavigationItem> GetActionItemsAsync()
	{
		List<MenuNode> actionItems = new List<MenuNode>();
		await foreach (MenuNode item in _menuAssembler.GetMenuHierarchyAsync("/global", 2))
		{
			if (IsActionItem(item))
			{
				actionItems.Add(item);
			}
		}
		foreach (MenuNode item2 in actionItems.OrderBy((MenuNode node) => node.Current.SortIndex))
		{
			yield return new NavigationItem
			{
				Name = HttpUtility.HtmlDecode(item2.Current.Text),
				Tooltip = HttpUtility.HtmlDecode(item2.Current.ToolTip),
				Url = item2.Current.Url,
				Path = item2.Current.Path,
				Target = item2.Current.Target,
				Children = (from child in item2.Children
					orderby child.Current.SortIndex
					select new NavigationItem
					{
						Name = HttpUtility.HtmlDecode(child.Current.Text),
						Url = child.Current.Url,
						Path = child.Current.Path,
						Target = child.Current.Target,
						Tooltip = HttpUtility.HtmlDecode(child.Current.ToolTip)
					}).ToList()
			};
		}
	}

	private async Task<string> GetUrlAsync(MenuNode node, bool global = false)
	{
		return HttpUtility.HtmlEncode(await GetUrlRecursiveAsync(node, global));
	}

	private async Task<string> GetUrlRecursiveAsync(MenuNode node, bool global)
	{
		string url = node.Current.Url;
		if (string.IsNullOrEmpty(url) || (global && url[0] == '#'))
		{
			foreach (MenuNode childNode in node.Children ?? new List<MenuNode>())
			{
				bool flag = childNode == null;
				if (!flag)
				{
					flag = !(await IsAvailableAsync(childNode));
				}
				if (!flag)
				{
					url = await GetUrlRecursiveAsync(childNode, global);
					if (!string.IsNullOrEmpty(url))
					{
						break;
					}
				}
			}
		}
		return url;
	}

	private async Task<NavigationItem> ToNavigationItemAsync(MenuNode node)
	{
		List<NavigationItem> children = new List<NavigationItem>();
		NavigationItem navigationItem2;
		NavigationItem navigationItem;
		foreach (MenuNode child in node.Children)
		{
			navigationItem = await ToNavigationItemAsync(child);
			navigationItem2 = navigationItem;
			navigationItem2.IsAvailable = await IsAvailableAsync(child);
			children.Add(navigationItem);
		}
		MenuItem current = node.Current;
		navigationItem2 = new NavigationItem
		{
			Name = HttpUtility.HtmlDecode(current.Text)
		};
		navigationItem = navigationItem2;
		navigationItem.Url = await GetUrlAsync(node);
		navigationItem2.Children = children;
		navigationItem2.Path = node.Current.Path;
		navigationItem2.IconName = node.Current.IconName;
		navigationItem2.CssClass = node.Current.CssClass;
		navigationItem2.IsEPiMenuItem = node.Current.IsEPiMenuItem;
		navigationItem2.Behavior = node.Current.Behavior;
		navigationItem2.Tooltip = HttpUtility.HtmlDecode(node.Current.ToolTip);
		return navigationItem2;
	}

	private async Task<ProductNavigationItem> ToProductNavigationItemAsync(MenuNode productNode)
	{
		string text = await GetUrlAsync(productNode, global: true);
		ProductNavigationItem product = new ProductNavigationItem
		{
			Id = productNode.Current.GetClientId(),
			Name = HttpUtility.HtmlDecode(productNode.Current.Text),
			Url = text,
			Tooltip = HttpUtility.HtmlDecode(productNode.Current.ToolTip)
		};
		List<NavigationItem> items = null;
		Uri uri = new Uri(text, UriKind.RelativeOrAbsolute);
		if (uri.IsAbsoluteUri && !string.Equals(_requestContext.HttpContext.Request.Host.Value, uri.Host, StringComparison.OrdinalIgnoreCase))
		{
			items = new List<NavigationItem>();
			foreach (MenuNode child in productNode.Children)
			{
				if (await IsAvailableAsync(child))
				{
					List<NavigationItem> list = items;
					list.Add(await ToNavigationItemAsync(child));
				}
			}
		}
		product.Children = items;
		return product;
	}

	private async Task<bool> IsAvailableAsync(MenuNode node)
	{
		if (node.Current.AuthorizationPolicy != null)
		{
			return (await _authorizationService.AuthorizeAsync(_requestContext.HttpContext.User, node.Current.AuthorizationPolicy)).Succeeded;
		}
		return node.Current.IsAvailable(_requestContext.HttpContext);
	}

	private static bool IsMainNavigation(MenuNode node)
	{
		bool num = node.Current.Alignment == MenuItemAlignment.Left;
		bool flag = node.Current.Path == "/global/more";
		if (num)
		{
			return !flag;
		}
		return false;
	}

	private static bool IsActionItem(MenuNode node)
	{
		bool num = node.Current.Alignment == MenuItemAlignment.Right;
		bool flag = node.Current.Path == "/global/more";
		bool flag2 = node.Current.Path == "/global/banner";
		if (num && !flag)
		{
			return !flag2;
		}
		return false;
	}
}

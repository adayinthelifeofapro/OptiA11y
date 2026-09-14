using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Navigation.Internal;

/// <summary>
///       Class used to assemble the site center menu
///       </summary>
public class MenuAssembler
{
	private readonly List<IMenuProvider> _menuProviders;

	private readonly IHttpContextAccessor _contextAccessor;

	private readonly ILogger<MenuAssembler> _log;

	public MenuAssembler(IEnumerable<IMenuProvider> menuProviders, IHttpContextAccessor httpContextAccessor, ILogger<MenuAssembler> log)
	{
		_menuProviders = menuProviders.ToList();
		_contextAccessor = httpContextAccessor;
		_log = log;
	}

	/// <summary>
	///       Creates a menu hierarchy and returns the nodes from below the given path uptil the supplied depth
	///       </summary>
	/// <param name="rootPath">Menu root path, e.g "/top".</param>
	/// <param name="relativeDepth">Menu depth, relative to the rootPath</param>
	/// <param name="cancellationToken">A cancellation token to abort the operation.</param>
	/// <returns>
	///       a List of top <see cref="T:EPiServer.Shell.Navigation.MenuNode" />s and theirs children.
	///       </returns>
	public virtual IAsyncEnumerable<MenuNode> GetMenuHierarchyAsync(string rootPath, int relativeDepth, CancellationToken cancellationToken = default(CancellationToken))
	{
		return GetMenuHierarchyAsync(rootPath, relativeDepth, string.Empty, cancellationToken);
	}

	/// <summary>
	///       Creates a menu hierarchy and returns the nodes from below the given path uptil the supplied depth
	///       </summary>
	/// <param name="rootPath">Menu root path, e.g "/top".</param>
	/// <param name="relativeDepth">Depth of the nodes to include in the result, relative to the rootPath depth</param>
	/// <param name="selectionPath">The menu path to select, If null the selection will be autoresolved</param>
	/// <param name="cancellationToken">A cancellation token to abort the operation.</param>
	/// <returns>
	///       a List of top <see cref="T:EPiServer.Shell.Navigation.MenuNode" />s and theirs children.
	///       </returns>
	public virtual async IAsyncEnumerable<MenuNode> GetMenuHierarchyAsync(string rootPath, int relativeDepth, string selectionPath, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		List<MenuNode> nodes = new List<MenuNode>();
		rootPath = rootPath.EnsureValidMenuPath();
		await foreach (MenuItem item2 in GetMenuItemsAsync(rootPath, relativeDepth, cancellationToken))
		{
			bool isSelected = (string.IsNullOrEmpty(selectionPath) ? item2.IsSelected(_contextAccessor.HttpContext) : item2.Path.Equals(selectionPath, StringComparison.OrdinalIgnoreCase));
			string parentPath = GetParentPath(item2);
			bool flag = false;
			foreach (MenuNode item3 in nodes)
			{
				MenuNode menuNode = item3.Find(parentPath);
				if (menuNode != null)
				{
					MenuNode item = new MenuNode(menuNode, item2)
					{
						IsSelected = isSelected
					};
					menuNode.Children.Add(item);
					menuNode.SortChildren();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (parentPath.CalculatePathDepth() > rootPath.CalculatePathDepth())
				{
					_log.CouldNotFindParentPath(parentPath);
					continue;
				}
				nodes.Add(new MenuNode(null, item2)
				{
					IsSelected = isSelected
				});
			}
		}
		foreach (MenuNode item4 in nodes)
		{
			item4.SortChildren();
		}
		nodes.Sort();
		foreach (MenuNode item5 in nodes)
		{
			yield return item5;
		}
	}

	/// <summary>
	///       Gets all menu items below a certain path until the given depth
	///       </summary>
	/// <param name="parentPath">The path.</param>
	/// <param name="relativeDepth">The max depth.</param>
	/// <param name="cancellationToken">A cancellation token to abort the operation.</param>
	/// <returns>
	/// </returns>
	internal virtual async IAsyncEnumerable<MenuItem> GetMenuItemsAsync(string parentPath, int relativeDepth, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		int maxDepth = relativeDepth;
		if (relativeDepth < int.MaxValue)
		{
			maxDepth = relativeDepth + parentPath.CalculatePathDepth();
		}
		parentPath = EnsureTrailingSlash(parentPath);
		List<MenuItem> allMenuItems = new List<MenuItem>();
		foreach (IMenuProvider provider in _menuProviders)
		{
			try
			{
				await foreach (MenuItem item in provider.GetMenuItemsAsync(cancellationToken))
				{
					item.IsEPiMenuItem = provider is IEPiProductMenuProvider;
					allMenuItems.Add(item);
				}
			}
			catch (Exception exception)
			{
				_log.FailedToAddMenuItem(exception);
			}
		}
		List<MenuItem> list = new List<MenuItem>();
		foreach (MenuItem menuItem in allMenuItems)
		{
			try
			{
				if (menuItem.Depth <= maxDepth && menuItem.Path.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) && !list.Any((MenuItem x) => x.Path.Equals(menuItem.Path, StringComparison.OrdinalIgnoreCase)))
				{
					list.Add(menuItem);
				}
			}
			catch (Exception exception2)
			{
				_log.FailedToAddMenuItem(exception2);
			}
		}
		foreach (MenuItem item2 in list.OrderBy((MenuItem m) => m.Depth))
		{
			yield return item2;
		}
	}

	private static string GetParentPath(MenuItem menuItem)
	{
		int num = menuItem.Path.LastIndexOf('/');
		if (num < 0)
		{
			num = 0;
		}
		return menuItem.Path.Substring(0, num);
	}

	private static string EnsureTrailingSlash(string parentPath)
	{
		if (!parentPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			parentPath += "/";
		}
		return parentPath;
	}
}

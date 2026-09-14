using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EPiServer.Framework.Localization;
using EPiServer.Framework.TypeScanner;
using EPiServer.Shell.Composition;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Navigation.Providers;

/// <summary>
///       Loads <see cref="T:EPiServer.Shell.Navigation.MenuItem" />s from attributed controllers and pages.
///       </summary>
internal class ReflectingMenuItemProvider : IMenuProvider
{
	private readonly ILogger<ReflectingMenuItemProvider> _log;

	private AttributeInfo<MenuAttributeBase>[] _reflectionCache;

	private readonly Dictionary<Type, Func<AttributeInfo<MenuAttributeBase>, MenuItem>> _menuFactories = new Dictionary<Type, Func<AttributeInfo<MenuAttributeBase>, MenuItem>>();

	private readonly ModuleTable _modules;

	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly IUrlHelperFactory _urlHelperFactory;

	private readonly LocalizationService _localizationService;

	private readonly ITypeScannerLookup _typeScannerLookup;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.Providers.ReflectingMenuItemProvider" /> class with specific configuration.
	///       This contructor is used for unit tests.
	///       </summary>
	/// <param name="moduleManager">The module manager.</param>
	/// <param name="httpContextAccessor">The http context accessor</param>
	/// <param name="urlHelperFactory">Used to create urls</param>
	/// <param name="localizationService">The service used for localization.</param>
	/// <param name="typeScannerLookup">The global type scanner repository</param>
	/// <param name="logger">The logger.</param>
	public ReflectingMenuItemProvider(ModuleTable moduleManager, IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory, LocalizationService localizationService, ITypeScannerLookup typeScannerLookup, ILogger<ReflectingMenuItemProvider> logger)
	{
		_modules = moduleManager;
		_httpContextAccessor = httpContextAccessor;
		_urlHelperFactory = urlHelperFactory;
		_localizationService = localizationService;
		_menuFactories[typeof(MenuItemAttribute)] = CreateMenuItem;
		_menuFactories[typeof(MenuSectionAttribute)] = CreateMenuSection;
		_typeScannerLookup = typeScannerLookup;
		_log = logger;
	}

	/// <summary>
	///       Probes plug-in assemblies for MVC controller methods and classes inheriting from
	///       decorated with the <see cref="T:EPiServer.Shell.Navigation.MenuItemAttribute" />.
	///       </summary>
	/// <returns>
	///       A list of <see cref="T:EPiServer.Shell.Navigation.MenuItem" />s found when probing assemblies.
	///       </returns>
	/// <remarks>
	///       The Provider implementation has to handle the security themself.
	///       </remarks>
	public IEnumerable<MenuItem> GetMenuItems()
	{
		AttributeInfo<MenuAttributeBase>[] array = _reflectionCache;
		if (array == null)
		{
			IEnumerable<AttributeInfo<MenuAttributeBase>> attributedMethods = _modules.GetModuleAssemblies().GetAttributedMethods<MenuAttributeBase>(_typeScannerLookup, new Type[1] { typeof(Controller) });
			IEnumerable<AttributeInfo<MenuAttributeBase>> attributedTypes = _modules.GetModuleAssemblies().GetAttributedTypes<MenuAttributeBase>(_typeScannerLookup, new Type[1] { typeof(Controller) });
			array = (_reflectionCache = attributedMethods.Union(attributedTypes).ToArray());
		}
		List<MenuItem> list = new List<MenuItem>();
		AttributeInfo<MenuAttributeBase>[] array2 = array;
		foreach (AttributeInfo<MenuAttributeBase> attributeInfo in array2)
		{
			if (!_menuFactories.ContainsKey(attributeInfo.Attribute.GetType()))
			{
				throw new InvalidOperationException("Couldn't find a menu item factory for the attribute type: " + attributeInfo.Attribute.GetType());
			}
			MenuItem item = _menuFactories[attributeInfo.Attribute.GetType()](attributeInfo);
			list.Add(item);
		}
		return list;
	}

	private SectionMenuItem CreateMenuSection(AttributeInfo<MenuAttributeBase> context)
	{
		string text = context.Attribute.GetLocalizedText(context.Attribute.TextResourceKey, context.Attribute.Text, _localizationService);
		if (string.IsNullOrEmpty(text))
		{
			text = TextFromTypeName(context.AttributedType);
		}
		SectionMenuItem sectionMenuItem = new SectionMenuItem(text, context.Attribute.MenuPath)
		{
			SortIndex = context.Attribute.SortIndex
		};
		ApplyAccessControl(context, sectionMenuItem);
		return sectionMenuItem;
	}

	private MenuItem CreateMenuItem(AttributeInfo<MenuAttributeBase> context)
	{
		MenuItem menuItem = (((object)context.AttributedMethod != null) ? CreateMenuItemForMethod(context) : CreateMenuItemForType(context));
		ApplyAccessControl(context, menuItem);
		return menuItem;
	}

	private void ApplyAccessControl(AttributeInfo<MenuAttributeBase> context, MenuItem menuItem)
	{
		ShellModule shellModule;
		if (HasAuthorizeAttribute(context.AttributedType))
		{
			menuItem.IsAvailable = IsAvailable(context.AttributedType);
		}
		else if (_modules.TryGetModule(context.AttributedType.Assembly, out shellModule))
		{
			menuItem.AuthorizationPolicy = shellModule.AuthorizationPolicy;
		}
	}

	private MenuItem CreateMenuItemForMethod(AttributeInfo<MenuAttributeBase> context)
	{
		MenuItemAttribute menuItemAttribute = (MenuItemAttribute)context.Attribute;
		string text = menuItemAttribute.GetLocalizedText(menuItemAttribute.TextResourceKey, menuItemAttribute.Text, _localizationService);
		if (string.IsNullOrEmpty(text))
		{
			text = context.AttributedMethod.Name;
		}
		string url = menuItemAttribute.Url;
		MenuItem menuItem = (string.IsNullOrEmpty(url) ? ((MenuItem)CreateRouteMenuItem(context.AttributedType, menuItemAttribute.MenuPath, context.AttributedMethod.Name, text)) : ((MenuItem)new UrlMenuItem(text, menuItemAttribute.MenuPath, url)));
		menuItem.CssClass = menuItemAttribute.CssClass;
		menuItem.SortIndex = menuItemAttribute.SortIndex;
		return menuItem;
	}

	private MenuItem CreateMenuItemForType(AttributeInfo<MenuAttributeBase> context)
	{
		MenuItemAttribute menuItemAttribute = (MenuItemAttribute)context.Attribute;
		string text = context.Attribute.GetLocalizedText(context.Attribute.TextResourceKey, context.Attribute.Text, _localizationService);
		string text2 = menuItemAttribute.GetResolvedUrl();
		if (string.IsNullOrEmpty(text))
		{
			text = TextFromTypeName(context.AttributedType);
		}
		MenuItem menuItem;
		if (string.IsNullOrEmpty(text2) && typeof(Controller).IsAssignableFrom(context.AttributedType))
		{
			menuItem = CreateRouteMenuItem(context.AttributedType, menuItemAttribute.MenuPath, null, text);
		}
		else
		{
			if (text2 != null && !text2.Contains(':') && !text2.StartsWith('/') && _modules.TryGetModule(context.AttributedType.Assembly, out var shellModule))
			{
				text2 = _modules.ResolvePath(shellModule.Name, text2);
			}
			menuItem = new UrlMenuItem(text, menuItemAttribute.MenuPath, text2);
		}
		menuItem.SortIndex = menuItemAttribute.SortIndex;
		menuItem.CssClass = menuItemAttribute.CssClass;
		return menuItem;
	}

	private string TextFromTypeName(Type type)
	{
		return TrimPrefixesAndSuffixes(type);
	}

	private string TrimPrefixesAndSuffixes(Type type)
	{
		if (_modules.TryGetModule(type.Assembly, out var shellModule))
		{
			return shellModule.GetRouteSegmentForController(type.Name);
		}
		return StringExtensions.TrimControllerSuffix(type.Name);
	}

	private RouteMenuItem CreateRouteMenuItem(Type controllerType, string path, string action, string text)
	{
		string text2 = TrimPrefixesAndSuffixes(controllerType);
		RouteValueDictionary routeValueDictionary = new RouteValueDictionary(new
		{
			controller = text2
		});
		if (!string.IsNullOrEmpty(action))
		{
			routeValueDictionary["action"] = action;
		}
		else
		{
			routeValueDictionary["action"] = "Index";
		}
		string text3 = null;
		if (_modules.TryGetModule(controllerType.Assembly, out var shellModule))
		{
			text3 = _modules.ResolvePath(shellModule.Name, text2 + "/" + action);
		}
		else
		{
			ActionContext context = new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
			RouteValueDictionary routeValueDictionary2 = new RouteValueDictionary();
			routeValueDictionary2["controller"] = text2;
			routeValueDictionary2["action"] = action;
			text3 = _urlHelperFactory.GetUrlHelper(context).Action(action, text2, routeValueDictionary2);
		}
		RouteMenuItem routeMenuItem = new RouteMenuItem(text, path, routeValueDictionary);
		if (!string.IsNullOrEmpty(text3))
		{
			routeMenuItem.Url = text3;
			_log.UsingMenuItemForAction(action, controllerType.FullName, text3);
		}
		else
		{
			_log.DidNotUseMenuItemForAction("Index", controllerType.FullName);
		}
		return routeMenuItem;
	}

	private bool HasAuthorizeAttribute(Type attributeProvider)
	{
		if (attributeProvider.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Length != 0)
		{
			return true;
		}
		return false;
	}

	private static Func<HttpContext, bool> IsAvailable(Type attributeProvider)
	{
		AuthorizeAttribute[] array = (AuthorizeAttribute[])attributeProvider.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);
		if (array.Length != 0)
		{
			return IsAvailable(array);
		}
		return (HttpContext request) => true;
	}

	private static Func<HttpContext, bool> IsAvailable(IEnumerable<AuthorizeAttribute> attributes)
	{
		return delegate(HttpContext request)
		{
			ClaimsPrincipal claimsPrincipal = request?.User;
			if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
			{
				return false;
			}
			foreach (AuthorizeAttribute attribute in attributes)
			{
				string[] array = attribute.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select((string s) => s.Trim()).ToArray() ?? Array.Empty<string>();
				if (array.Length != 0 && !array.Any(claimsPrincipal.IsInRole))
				{
					return false;
				}
			}
			return true;
		};
	}
}

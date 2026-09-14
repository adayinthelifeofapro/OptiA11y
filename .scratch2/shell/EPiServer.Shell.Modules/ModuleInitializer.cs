using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using EPiServer.DependencyInjection;
using EPiServer.Framework.Hosting;
using EPiServer.Framework.TypeScanner;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules.Internal;
using EPiServer.Shell.Routing;
using EPiServer.Shell.Services.Rest;
using EPiServer.Shell.ViewComposition;
using EPiServer.Shell.Web.Mvc;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Matching.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPiServer.Shell.Modules;

/// <summary>
///       This class whole purpose in life is to initializes modules while being internally used by EPiServer.
///       </summary>
public class ModuleInitializer
{
	private readonly ILogger<ModuleInitializer> _log;

	private readonly ProtectedModuleOptions _protectedOptions;

	private readonly IFallbackModuleController _fallbackModuleController;

	private readonly IEnumerable<IViewProvider> _viewProviders;

	private readonly ICompositeFileProvider _compositeFileProvider;

	private readonly IWebHostEnvironment _webHostEnvironment;

	private readonly ILoggerFactory _loggerFactory;

	private readonly IOptionsMonitor<StaticFileOptions> _staticFileOptionsAccessor;

	private readonly string[] _verbs = new string[4] { "Get", "Post", "Put", "Delete" };

	private readonly (string AnonymousPath, string CommunicationScript, string RenderPropertyScript) _opeResources = (AnonymousPath: "/Util/javascript/", CommunicationScript: "communicationInjector.js", RenderPropertyScript: "deliveryPropertyRenderer.js");

	private readonly ApplicationPartManager _applicationPartManager;

	private readonly ITypeScannerLookup _typeScanner;

	/// <summary>
	///       Creates a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleInitializer" /> class.
	///       </summary>
	public ModuleInitializer(ProtectedModuleOptions protectedOptions, ICompositeFileProvider compositeFileProvider, IWebHostEnvironment webHostEnvironment, IFallbackModuleController fallbackModuleController, IEnumerable<IViewProvider> viewProviders, ILoggerFactory loggerFactory, IOptionsMonitor<StaticFileOptions> staticFileOptionsAccessor, ApplicationPartManager applicationPartManager, ITypeScannerLookup typeScanner, ILogger<ModuleInitializer> logger)
	{
		_protectedOptions = protectedOptions;
		_fallbackModuleController = fallbackModuleController;
		_viewProviders = viewProviders;
		_compositeFileProvider = compositeFileProvider;
		_webHostEnvironment = webHostEnvironment;
		_loggerFactory = loggerFactory;
		_staticFileOptionsAccessor = staticFileOptionsAccessor;
		_applicationPartManager = applicationPartManager;
		_typeScanner = typeScanner;
		_log = logger;
	}

	/// <summary>
	///       Registers route table and view engine related to the module
	///       </summary>
	/// <param name="modules">The modules that will be registered.</param>
	/// <param name="routeBuilder">
	/// </param>
	internal void RegisterModules(IEnumerable<ShellModule> modules, IEndpointRouteBuilder routeBuilder)
	{
		foreach (ShellModule module in modules)
		{
			RegisterServerRoutes(module, routeBuilder);
			RegisterViewRoutes(routeBuilder, module);
			RegisterViewComponents(module);
		}
	}

	/// <summary>
	///       Registers dojo localization routes for registered dojo modules
	///       </summary>
	/// <param name="modules">A collection of registered shell modules</param>
	/// <param name="routeBuilder">
	/// </param>
	internal void RegisterClientResourceRoutes(IEnumerable<ShellModule> modules, IEndpointRouteBuilder routeBuilder)
	{
		ShellModule shellModuleNamedShell = modules.FirstOrDefault((ShellModule m) => m.Name == "Shell");
		VirtualPathUtilityEx.ToAppRelative(_protectedOptions.RootPath);
		foreach (ShellModule module in modules)
		{
			string text = VirtualPathUtilityEx.ToAppRelative(module.ClientResourcePath);
			string clientResourcePath = VirtualPathUtilityEx.ToAbsolute(module.ClientResourcePath);
			if (!module.IsAppModule())
			{
				string url = VirtualPathUtilityEx.AppendTrailingSlash(text.TrimStart(new char[2] { '~', '/' })) + "{folder}/{**path:epi-staticfile}";
				IEndpointConventionBuilder builder = MapStaticFiles(module.Name, url, routeBuilder);
				if (!module.IsPublic)
				{
					builder.RequireClientShellAuthorization(module);
				}
			}
			foreach (DojoPath dojoModule in module.Manifest.DojoModules)
			{
				RegisterNlsRoute(dojoModule.Name, dojoModule.Path, shellModuleNamedShell, module, text, routeBuilder);
			}
			DojoConfiguration dojo = module.Manifest.Dojo;
			if (dojo == null)
			{
				continue;
			}
			foreach (DojoPath path in dojo.Paths)
			{
				RegisterNlsRoute(path.Name, path.Path, shellModuleNamedShell, module, text, routeBuilder);
			}
			foreach (DojoPackage package in dojo.Packages)
			{
				RegisterNlsRoute(package.Name, package.Location, shellModuleNamedShell, module, text, routeBuilder);
			}
			RegisterVersionAgnosticRoute(module, clientResourcePath, routeBuilder);
		}
	}

	/// <summary>
	///       Registers a route which redirects the urls of js file which contains "/latest/"
	///       </summary>
	/// <param name="module">
	/// </param>
	/// <param name="clientResourcePath">
	/// </param>
	/// <param name="routeBuilder">
	/// </param>
	private void RegisterVersionAgnosticRoute(ShellModule module, string clientResourcePath, IEndpointRouteBuilder routeBuilder)
	{
		if (module.Name.Equals("CMS"))
		{
			string text = VirtualPathUtility.Combine(VirtualPathUtilityEx.ToAppRelative(module.ResourceBasePath), "latest");
			text = VirtualPathUtilityEx.AppendTrailingSlash(text) + "{*pathInfo:regex(.+\\.(css|js)$)}";
			routeBuilder.MapGet(text, async delegate(HttpContext context)
			{
				string requestedFile = context.GetRouteData().Values["pathInfo"] as string;
				string location = BuildVersionAgnosticRoute(clientResourcePath, requestedFile);
				context.Response.Redirect(location);
				await Task.CompletedTask;
			});
		}
	}

	/// <summary>
	///       Builds a version agnostic route with consideration to specific scenario for scripts
	///       that need to be redirected to anonymous folder (currently for decoupled sites).
	///       </summary>
	/// <param name="clientResourcePath">
	/// </param>
	/// <param name="requestedFile">
	/// </param>
	/// <returns>Route using clientResourcePath or anonymous path</returns>
	private string BuildVersionAgnosticRoute(string clientResourcePath, string requestedFile)
	{
		if (!string.IsNullOrEmpty(requestedFile))
		{
			int num = requestedFile.IndexOf(_opeResources.CommunicationScript, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				num = requestedFile.IndexOf(_opeResources.RenderPropertyScript, StringComparison.OrdinalIgnoreCase);
			}
			if (num >= 0)
			{
				clientResourcePath = _opeResources.AnonymousPath;
				requestedFile = requestedFile.Substring(num);
			}
		}
		return VirtualPathUtility.Combine(clientResourcePath ?? string.Empty, requestedFile ?? string.Empty);
	}

	internal IEndpointConventionBuilder MapStaticFiles(string staticFileOptionName, string url, IEndpointRouteBuilder routeBuilder, StaticFileOptions fileOptions = null)
	{
		StaticFileOptions staticFileOptions = fileOptions;
		if (staticFileOptions == null)
		{
			staticFileOptions = _staticFileOptionsAccessor.Get(staticFileOptionName);
			staticFileOptions.FileProvider = (IFileProvider?)_compositeFileProvider;
		}
		StaticFileMiddleware staticFileMiddleware = new StaticFileMiddleware(delegate(HttpContext c)
		{
			c.Response.StatusCode = 404;
			return Task.CompletedTask;
		}, _webHostEnvironment, Options.Create(staticFileOptions), _loggerFactory);
		return routeBuilder.MapGet(url, async delegate(HttpContext context)
		{
			context.SetEndpoint(null);
			await staticFileMiddleware.Invoke(context);
		});
	}

	private void RegisterNlsRoute(string name, string path, ShellModule shellModuleNamedShell, ShellModule module, string clientResourcePath, IEndpointRouteBuilder routeBuilder)
	{
		if (!new Uri(path, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
		{
			string text = UriUtil.Combine(clientResourcePath, path).TrimStart(new char[2] { '~', '/' });
			text = VirtualPathUtilityEx.AppendTrailingSlash(text) + "{**pathInfo:epi-dojo-nls}";
			RouteValueDictionary defaults = new RouteValueDictionary(new
			{
				controller = "EPiResources",
				action = "DojoResources",
				module = module,
				area = shellModuleNamedShell?.Name
			});
			RouteValueDictionary dataTokens = new RouteValueDictionary(new
			{
				module = shellModuleNamedShell,
				shellModule = module
			});
			routeBuilder.MapControllerRoute(name, text, defaults, null, dataTokens).RequireShellAuthorization(module);
		}
	}

	private void RegisterServerRoutes(ShellModule shellModule, IEndpointRouteBuilder routeBuilder)
	{
		ArgumentNullException.ThrowIfNull(shellModule, "shellModule");
		IViewManager viewManager = routeBuilder.ServiceProvider.GetRequiredService<IViewManager>();
		foreach (RouteDescription route in shellModule.Manifest.Routes)
		{
			RouteDescription routeDescription = route;
			RouteValueDictionary defaults = new RouteValueDictionary(new
			{
				action = "Index",
				module = shellModule
			});
			Type[] array = (from t in shellModule.Assemblies.SelectMany((Assembly a) => a.GetTypes())
				where !t.IsAbstract && typeof(Controller).IsAssignableFrom(t) && t.GetCustomAttributes(typeof(RestStoreAttribute), inherit: true).Length == 0
				select t).ToArray();
			IEnumerable<Type> enumerable = array.Where((Type t) => typeof(RestControllerBase).IsAssignableFrom(t));
			Type type = null;
			KeyValueElement defaultController = routeDescription.Defaults.FirstOrDefault((KeyValueElement e) => string.Equals("controller", e.Key, StringComparison.OrdinalIgnoreCase));
			if (defaultController != null)
			{
				type = array.FirstOrDefault((Type c) => defaultController.Value.Equals(c.Name, StringComparison.OrdinalIgnoreCase) || defaultController.Value.Equals(c.Name.Replace("Controller", string.Empty, StringComparison.OrdinalIgnoreCase)));
				if ((object)type == null)
				{
					defaults["routeController"] = defaultController.Value;
					defaultController = new KeyValueElement("controller", "DefaultShellModule");
				}
			}
			foreach (Type item in enumerable)
			{
				string[] verbs = _verbs;
				foreach (string text in verbs)
				{
					RouteValueDictionary defaults2 = new RouteValueDictionary(new
					{
						module = shellModule,
						action = text,
						controller = item.Name.Replace("Controller", string.Empty)
					});
					RouteValueDictionary constraints = new RouteValueDictionary(new
					{
						httpMethod = new HttpMethodRouteConstraint(text)
					});
					MapModuleControllerRoute(routeBuilder, item, shellModule, $"{shellModule.Name}_{text}_{item.Name}", $"{shellModule.RouteBasePath}{shellModule.Name}/{item.Name.Replace("Controller", string.Empty)}/{{action}}", defaults2, constraints);
				}
			}
			string controller = ((defaultController == null) ? string.Empty : defaultController.Value);
			List<Type> list = array.Except(enumerable).ToList();
			string text2 = (shellModule.RouteBasePath + routeDescription.Url).Replace("{moduleArea}", shellModule.Name);
			string controllerPrefix = shellModule.Manifest.Route.ControllerPrefix;
			foreach (Type item2 in list)
			{
				if ((!string.IsNullOrWhiteSpace(controllerPrefix) && item2.Name.StartsWith(controllerPrefix, StringComparison.OrdinalIgnoreCase)) || item2.GetCustomAttributes(typeof(NonAreaAttribute), inherit: true).Length != 0)
				{
					string name = item2.Name;
					name = name.Replace("Controller", "");
					string newValue = name;
					if (!string.IsNullOrWhiteSpace(controllerPrefix) && name.StartsWith(controllerPrefix, StringComparison.OrdinalIgnoreCase))
					{
						newValue = name.Substring(controllerPrefix.Length);
					}
					string url = text2.Replace("{controller}", newValue);
					RegisterServerRoute(url, name, item2);
				}
			}
			RegisterServerRoute(text2, controller, type);
			void RegisterServerRoute(string url2, string value, Type controllerType)
			{
				ModuleAreaConstraint moduleAreaConstraint = new ModuleAreaConstraint(shellModule);
				RouteValueDictionary constraints2 = new RouteValueDictionary(new
				{
					controller = new ControllerNameRouteConstraint(shellModule, viewManager),
					moduleArea = moduleAreaConstraint,
					module = moduleAreaConstraint
				});
				RouteValueDictionary dataTokens = new RouteValueDictionary(new
				{
					module = shellModule
				});
				Merge(defaults, routeDescription.Defaults);
				if (!string.IsNullOrEmpty(value))
				{
					defaults["controller"] = value;
				}
				MapModuleControllerRoute(routeBuilder, controllerType, shellModule, Guid.NewGuid().ToString("N"), url2, new RouteValueDictionary(defaults), constraints2, dataTokens);
			}
		}
	}

	private static void Merge(RouteValueDictionary dictionary, IList<KeyValueElement> values)
	{
		foreach (KeyValueElement value in values)
		{
			dictionary[value.Key] = value.Value;
		}
	}

	internal void RegisterRestStores(IEnumerable<ShellModule> modules, IEnumerable<Type> restStoreTypes, IEndpointRouteBuilder routeBuilder)
	{
		foreach (Type restStoreType in restStoreTypes)
		{
			ShellModule shellModule = modules.FirstOrDefault((ShellModule m) => m.Assemblies.Contains(restStoreType.Assembly));
			if (shellModule == null)
			{
				continue;
			}
			foreach (RestStoreAttribute customAttribute in ((MemberInfo)restStoreType).GetCustomAttributes<RestStoreAttribute>(true))
			{
				if (restStoreType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
				{
					throw new SecurityException("Stores ending with Controller is not supported because they might be executed as regular MVC controllers and bypass security");
				}
				routeBuilder.Map($"{shellModule.RouteBasePath}{shellModule.Name}/{"stores"}/{customAttribute.StoreName}/{{*id}}", (HttpContext next) => Task.CompletedTask).WithMetadata(new MethodOverrideEndpointMetadata(), new SuppressLinkGenerationMetadata());
				MapModuleControllerRoute(routeBuilder, restStoreType, shellModule, shellModule.Name + "_Store_" + customAttribute.StoreName + "_action", $"{shellModule.RouteBasePath}{shellModule.Name}/{"stores"}/{customAttribute.StoreName}/{{action}}", new RouteValueDictionary(new
				{
					controller = restStoreType.Name,
					module = shellModule
				}));
				string[] verbs = _verbs;
				foreach (string text in verbs)
				{
					MapModuleControllerRoute(routeBuilder, restStoreType, shellModule, $"{shellModule.Name}_Store_{customAttribute.StoreName}_{text}", $"{shellModule.RouteBasePath}{shellModule.Name}/{"stores"}/{customAttribute.StoreName}/{{*id}}", new RouteValueDictionary(new
					{
						controller = restStoreType.Name,
						action = text,
						module = shellModule
					}), new
					{
						httpMethod = new HttpMethodRouteConstraint(text)
					});
				}
			}
		}
	}

	private void MapModuleControllerRoute(IEndpointRouteBuilder routeBuilder, Type controllerType, ShellModule module, string name, string url, RouteValueDictionary defaults, object constraints = null, object dataTokens = null)
	{
		if (!module.IsAppModule() && ((object)controllerType == null || controllerType.GetCustomAttributes(typeof(NonAreaAttribute), inherit: true).Length == 0))
		{
			if ("DefaultShellModule".Equals(defaults["controller"]))
			{
				string pattern = module.RouteBasePath + module.Name;
				routeBuilder.MapControllerRoute("default" + name, pattern, new RouteValueDictionary(defaults), constraints, dataTokens).RequireShellAuthorization(module);
			}
			url = url.Replace("/" + module.Name + "/", "/{area}/");
			defaults["area"] = module.Name;
			routeBuilder.MapAreaControllerRoute(name, module.Name, url, defaults, constraints, dataTokens).RequireShellAuthorization(module);
		}
		else
		{
			routeBuilder.MapControllerRoute(name, url, defaults, constraints, dataTokens).RequireShellAuthorization(module);
		}
		_log.AddingRoute(url, module.Name);
	}

	private void RegisterViewRoutes(IEndpointRouteBuilder endpointRouteBuilder, ShellModule module)
	{
		foreach (IRoutable item in _viewProviders.SelectMany((IViewProvider provider) => from x in provider.GetViews().OfType<IRoutable>()
			where module.Assemblies.Contains(((object)x).GetType().Assembly)
			select x))
		{
			RouteValueDictionary defaults = new RouteValueDictionary(new
			{
				controller = _fallbackModuleController.ControllerType.Name.Replace("Controller", string.Empty),
				module = module,
				routeController = item.RouteSegment,
				action = "index"
			});
			MapModuleControllerRoute(endpointRouteBuilder, _fallbackModuleController.ControllerType, module, module.Name + "_" + item.RouteSegment, module.RouteBasePath + module.Name + "/" + item.RouteSegment, defaults);
		}
	}

	private void RegisterViewComponents(ShellModule module)
	{
		List<AssemblyElement> list = module?.Manifest?.Assemblies;
		if (list.Count == 0)
		{
			return;
		}
		foreach (AssemblyElement manifestAssembly in list)
		{
			Assembly assembly = module.Assemblies.FirstOrDefault((Assembly p) => p.GetName().Name == manifestAssembly.Assembly);
			if ((object)assembly != null)
			{
				ITypeScannerLookup typeScanner = _typeScanner;
				IEnumerable<Type> enumerable = ((typeScanner == null) ? null : typeScanner.AllTypes?.Where((Type t) => typeof(ViewComponent).IsAssignableFrom(t) && t.Assembly.GetName().FullName == assembly.GetName().FullName));
				if (enumerable.Any())
				{
					_applicationPartManager.ApplicationParts.Add(new ViewComponentApplicationPart(assembly.GetName(), enumerable));
				}
			}
		}
	}
}

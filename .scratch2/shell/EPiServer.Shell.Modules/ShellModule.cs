using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EPiServer.Framework.TypeScanner;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Configuration;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Represents a module of the EPiServer Framework application
///       </summary>
public class ShellModule
{
	private readonly ILogger<ShellModule> _log;

	private IEnumerable<Type> _controllers;

	[CompilerGenerated]
	private string _003CClientResourcePath_003Ek__BackingField;

	/// <summary>
	///       Read only shell module policy
	///       </summary>
	public const string ReadOnlyShellModulePolicy = "ReadOnlyShellModulePolicy";

	internal ITypeScannerLookup TypeScannerLookup { get; set; }

	internal IFileProvider FileProvider { get; set; }

	/// <summary>
	///       The name of the module, typically the same as the directory where the module is located.
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       Controllers associated with this module
	///       </summary>
	public IEnumerable<Type> Controllers
	{
		get
		{
			return _controllers ?? (_controllers = GetControllers());
		}
		set
		{
			_controllers = value;
		}
	}

	/// <summary>
	///       Assemblies associated with this module. These are the combination of dll
	///       files in the module's bin directory and assemblies configured in the module's manifest
	///       </summary>
	public IEnumerable<Assembly> Assemblies { get; set; } = Array.Empty<Assembly>();

	/// <summary>
	///       Describes the module. This information is de-serialized from a the file module.config located in the module directory.
	///       </summary>
	public ShellModuleManifest Manifest { get; set; } = new ShellModuleManifest();

	/// <summary>
	///       An absolute path from which routes to this module are resolved. For instance "episerver/yourmodule/".
	///       </summary>
	public string RouteBasePath { get; set; }

	/// <summary>
	///       The path to the module directory. Views and client resources are located below this folder.
	///       </summary>
	public string ResourceBasePath { get; set; }

	/// <summary>
	///       Gets or sets the client resource path.
	///       </summary>
	public string ClientResourcePath
	{
		get
		{
			return _003CClientResourcePath_003Ek__BackingField ?? ResourceBasePath;
		}
		set
		{
			_003CClientResourcePath_003Ek__BackingField = (string.IsNullOrEmpty(value) ? null : value);
		}
	}

	/// <summary>
	///       Authorization policy name. If not set for protected modules, only web admins and web editors can interact with this module.
	///       </summary>
	public virtual string AuthorizationPolicy => Manifest?.AuthorizationPolicy;

	/// <summary>
	///       Client Authorization policy name. If not set for protected modules, only web admins and web editors can interact with this module.
	///       </summary>
	public virtual string ClientAuthorizationPolicy => Manifest?.ClientAuthorizationPolicy;

	/// <summary>
	///       Determines whether the module is public
	///       </summary>
	public bool IsPublic { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ShellModule" /> class.
	///       </summary>
	/// <param name="logger">The logger.</param>
	public ShellModule(ILogger<ShellModule> logger = null)
	{
		_log = logger;
	}

	/// <summary>
	///       Gets the resolved route base path.
	///       </summary>
	/// <returns>A path in a format that is absolute and includes a virtual directory path if present. For instance "/localhost/mysite/episerver/mymodule".</returns>
	public string GetResolvedRouteBasePath()
	{
		if (string.IsNullOrEmpty(RouteBasePath))
		{
			return "/";
		}
		return VirtualPathUtilityEx.AppendTrailingSlash("/" + RouteBasePath);
	}

	/// <summary>
	///       Gets a controller type by the controller name.
	///       </summary>
	/// <param name="controllerName">Name of the controller.</param>
	/// <returns>A type if a matching controller is fount; otherwise null</returns>
	public Type GetControllerType(string controllerName)
	{
		foreach (RouteDescription route in Manifest.Routes)
		{
			Type type = Controllers.FirstOrDefault(GetCompareExpression(route.ControllerPrefix, controllerName));
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}

	public string GetMappedControllerName(string controllerName)
	{
		Type controllerType = GetControllerType(controllerName);
		if (!(controllerType != null))
		{
			return null;
		}
		if (!controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
		{
			return controllerType.Name;
		}
		string name = controllerType.Name;
		return name.Substring(0, name.Length - 10);
	}

	/// <summary>
	///       Gets the route for the given controller name.
	///       </summary>
	/// <param name="controllerName">The controller name</param>
	/// <returns>The route for the controller or null</returns>
	public RouteDescription GetRouteForController(string controllerName)
	{
		foreach (RouteDescription route in Manifest.Routes)
		{
			if (Controllers.FirstOrDefault(GetCompareExpression(route.ControllerPrefix, controllerName)) != null)
			{
				return route;
			}
		}
		return null;
	}

	/// <summary>
	///       Gets the route segement used for creating URLs; taking into account the route prefix.
	///       </summary>
	/// <param name="controllerName">The controller name</param>
	/// <returns>The route segement</returns>
	/// <example>
	///       For "FooController" it will return "Foo".
	///       </example>
	public string GetRouteSegmentForController(string controllerName)
	{
		return (GetRouteForController(controllerName) ?? new RouteDescription()).TrimControllerName(controllerName);
	}

	private static Func<Type, bool> GetCompareExpression(string prefix, string controllerName)
	{
		if (!controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
		{
			controllerName += "Controller";
		}
		if (string.IsNullOrEmpty(prefix))
		{
			return (Type c) => c.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase);
		}
		return (Type c) => c.Name.Equals(controllerName, StringComparison.OrdinalIgnoreCase) || c.Name.Equals(prefix + controllerName, StringComparison.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		if (!(obj is ShellModule shellModule))
		{
			return false;
		}
		return Name == shellModule.Name;
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return Name.GetHashCode();
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return "[ShellModule Name='" + Name + "' RouteBasePath='" + RouteBasePath + "' ResourceBasePath='" + ResourceBasePath + "' ClientResourcePath='" + ClientResourcePath + "' Assemblies=[" + string.Join(", ", Assemblies.Select((Assembly a) => a.GetName().Name).ToArray()) + "] Controllers=" + ((_controllers != null) ? ("[" + string.Join(", ", _controllers.Select((Type c) => c.Name).ToArray()) + "]") : "undefined") + "]";
	}

	/// <summary>
	///       Creates the view module for this module.
	///       </summary>
	/// <param name="moduleTable">The module table.</param>
	/// <param name="service">The client resource service.</param>
	/// <returns>An object that will be serialized and sent to the client when initializing a view.</returns>
	public virtual ModuleViewModel CreateViewModel(ModuleTable moduleTable, IClientResourceService service)
	{
		return new ModuleViewModel(this, service);
	}

	/// <summary>
	///       Scans the module's assemblies for controller types
	///       </summary>
	/// <returns>
	/// </returns>
	private IList<Type> GetControllers()
	{
		return (from t in TypeScannerLookup.AllTypes
			where Assemblies.Any((Assembly a) => a == t.Assembly)
			where typeof(Controller).IsAssignableFrom(t)
			where !t.IsAbstract
			where !t.IsGenericType
			select t).ToList();
	}

	/// <summary>
	///       Get the webhelp url for this module
	///       </summary>
	/// <returns>A string containing the webhelp url </returns>
	public virtual string GetHelpUrl()
	{
		string text = Manifest.HelpFile;
		if (!string.IsNullOrWhiteSpace(text))
		{
			Assembly assembly = Assemblies.FirstOrDefault();
			if (assembly != null)
			{
				Version version = assembly.GetName().Version;
				text = text.Replace("{major}", version.Major.ToString(CultureInfo.InvariantCulture));
				text = text.Replace("{minor}", version.Minor.ToString(CultureInfo.InvariantCulture));
				text = text.Replace("{build}", version.Build.ToString(CultureInfo.InvariantCulture));
				text = text.Replace("{revision}", version.Revision.ToString(CultureInfo.InvariantCulture));
			}
			return GetCultureSpecificHelpUrl(text);
		}
		return null;
	}

	private string GetCultureSpecificHelpUrl(string url)
	{
		string text = "{culture}";
		if (!url.Contains(text))
		{
			return url;
		}
		string twoLetterISOLanguageName = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		string text2 = url.Replace(text, twoLetterISOLanguageName);
		if (!new Uri(text2, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
		{
			text2 = ModuleTable.ResolvePath(this, text2);
			if (twoLetterISOLanguageName != "en" && !FileProvider.GetFileInfo(text2.Split('?')[0]).Exists)
			{
				text2 = url.Replace(text, "en");
				text2 = ModuleTable.ResolvePath(this, text2);
				_log?.CouldNotFindFileForCulture(text2, twoLetterISOLanguageName);
			}
		}
		return text2;
	}

	/// <summary>
	///       Merge the module
	///       </summary>
	/// <param name="module">
	/// </param>
	/// <exception cref="T:System.NotImplementedException">
	/// </exception>
	public void Merge(ShellModule module)
	{
		if (!Equals(module))
		{
			throw new ArgumentException("Only equal ShellModules can be merged.");
		}
		RouteBasePath = (string.IsNullOrEmpty(RouteBasePath) ? module.RouteBasePath : RouteBasePath);
		ResourceBasePath = (string.IsNullOrEmpty(ResourceBasePath) ? module.ResourceBasePath : ResourceBasePath);
		Assemblies = Assemblies.Union(module.Assemblies);
	}

	/// <summary>
	///       Merges duplicate modules in the specified modules list list
	///       </summary>
	/// <param name="modules">
	/// </param>
	/// <returns>
	/// </returns>
	/// <exception cref="T:System.InvalidOperationException">
	/// </exception>
	public static IEnumerable<ShellModule> MergeDuplicateModules(IEnumerable<ShellModule> modules)
	{
		List<ShellModule> list = modules.ToList();
		List<ShellModule> list2 = list.Distinct().ToList();
		if (list.Count != list2.Count)
		{
			List<ShellModule> list3 = new List<ShellModule>();
			foreach (IGrouping<ShellModule, ShellModule> item in from m in list
				group m by m into g
				where g.Count() > 1
				select g)
			{
				ShellModule shellModule = item.FirstOrDefault((ShellModule m) => !string.IsNullOrEmpty(m.Manifest.Type)) ?? item.Key;
				foreach (ShellModule item2 in item)
				{
					shellModule.Merge(item2);
				}
				list3.Add(shellModule);
			}
			list2 = list2.Except(list3).Union(list3).ToList();
		}
		return list2;
	}
}

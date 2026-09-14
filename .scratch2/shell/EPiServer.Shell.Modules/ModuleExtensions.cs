using System;
using System.Linq;
using System.Reflection;
using EPiServer.Shell.Configuration;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Helps in dealing with shell modules
///       </summary>
public static class ModuleExtensions
{
	/// <summary>
	///       Gets a shell module from the route data tokens.
	///       </summary>
	/// <param name="routeData">
	/// </param>
	/// <returns>
	/// </returns>
	public static ShellModule GetModule(this RouteData routeData)
	{
		return routeData.DataTokens["module"] as ShellModule;
	}

	/// <summary>
	///       Ensures that the manifest has at least one route; if it doesn't the default is added.
	///       </summary>
	internal static void EnsureDefaultRoute(this ShellModuleManifest manifest, string moduleName)
	{
		if (manifest.Routes.Count == 0)
		{
			RouteDescription routeDescription = new RouteDescription();
			if (!string.IsNullOrEmpty(moduleName))
			{
				routeDescription.Constraints.Add(new KeyValueElement
				{
					Key = "moduleArea",
					Value = moduleName
				});
			}
			routeDescription.Defaults.Add(new KeyValueElement
			{
				Key = "action",
				Value = "index"
			});
			routeDescription.Defaults.Add(new KeyValueElement
			{
				Key = "id",
				Value = ""
			});
			manifest.Routes.Add(routeDescription);
		}
	}

	/// <summary>
	///       Resolve version for shell module
	///       </summary>
	/// <param name="module">
	/// </param>
	/// <returns>
	/// </returns>
	public static Version ResolveVersion(this ShellModule module)
	{
		if (!string.IsNullOrEmpty(module.Manifest.Version) && Version.TryParse(module.Manifest.Version, out Version result))
		{
			return result;
		}
		Assembly assembly = module.Assemblies.FirstOrDefault((Assembly a) => a.GetName().Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)) ?? module.Assemblies.FirstOrDefault();
		if (!(assembly != null))
		{
			return new Version();
		}
		return assembly.GetName().Version;
	}
}

using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Used to parse <see cref="T:EPiServer.Shell.Modules.ShellModule" /> and <see cref="T:EPiServer.Shell.Configuration.ShellModuleManifest" /> into a format that
///       is suitable for client consumption.
///       </summary>
public class ModuleViewModel
{
	/// <summary>
	///       The name of the module
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       Module initializer class.
	///       </summary>
	public string Initializer { get; set; }

	/// <summary>
	///       The module dependencies to load before this one.
	///        </summary>
	public ICollection<ModuleDependencyViewModel> ModuleDependencies { get; private set; }

	/// <summary>
	///       The CSS resources needed by the module.
	///       </summary>
	public ICollection<string> CssResources { get; private set; }

	/// <summary>
	///       The script resources needed by the module.
	///       </summary>
	public ICollection<string> ScriptResources { get; private set; }

	/// <summary>
	///       The available routes.
	///       </summary>
	public IList<ModuleRoutePair> Routes { get; private set; }

	/// <summary>
	///       The path to the web help
	///       </summary>
	public string HelpPath { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleViewModel" /> class.
	///       </summary>
	/// <param name="module">The module.</param>
	/// <param name="clientResourceService">The client resource service.</param>
	public ModuleViewModel(ShellModule module, IClientResourceService clientResourceService)
	{
		Name = module.Name;
		if (module.Manifest.ClientModule != null)
		{
			Initializer = module.Manifest.ClientModule.Initializer;
			ModuleDependencies = module.Manifest.ClientModule.ModuleDependencies.Select((ModuleDependency d) => new ModuleDependencyViewModel
			{
				ModuleName = d.Dependency,
				DependencyType = (int)((d.DependencyType == ModuleDependencyTypes.None) ? ModuleDependencyTypes.Require : d.DependencyType)
			}).ToList();
		}
		CssResources = GetResources((ClientResourceType)2, module, clientResourceService);
		ScriptResources = GetResources((ClientResourceType)0, module, clientResourceService);
		HelpPath = module.GetHelpUrl();
		AddRouteInformation(module);
	}

	/// <summary>
	///       Adds the route information.
	///       </summary>
	/// <param name="module">The module.</param>
	private void AddRouteInformation(ShellModule module)
	{
		Routes = new List<ModuleRoutePair>();
		foreach (RouteDescription route in module.Manifest.Routes)
		{
			Dictionary<string, string> dictionary = route.Defaults.ToDictionary((KeyValueElement e) => e.Key, (KeyValueElement e) => e.Value);
			if (!dictionary.ContainsKey("moduleArea"))
			{
				dictionary.Add("moduleArea", module.Name);
			}
			string routeBasePath = module.GetResolvedRouteBasePath() + route.Url;
			Routes.Add(new ModuleRoutePair(routeBasePath, dictionary));
		}
	}

	/// <summary>
	///       Gets the resources.
	///       </summary>
	/// <param name="resourceType">Type of the resource to return.</param>
	/// <param name="module">The module.</param>
	/// <param name="clientResourceService">The client resource service.</param>
	/// <returns>
	/// </returns>
	private static List<string> GetResources(ClientResourceType resourceType, ShellModule module, IClientResourceService clientResourceService)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected I4, but got Unknown
		List<string> list = new List<string>();
		if (module.Manifest.ClientModule != null)
		{
			foreach (string item in module.Manifest.ClientModule.RequiredResources.Select((ClientResourceReference rl) => rl.Name))
			{
				list.AddRange(from r in clientResourceService.GetClientResources(item, (ClientResourceType[])(object)new ClientResourceType[1] { (ClientResourceType)(int)resourceType })
					select r.Path);
			}
		}
		return list;
	}
}

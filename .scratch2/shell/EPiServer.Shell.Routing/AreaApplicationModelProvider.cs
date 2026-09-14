using System.Linq;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace EPiServer.Shell.Routing;

internal class AreaApplicationModelProvider : IApplicationModelProvider
{
	private readonly ModuleTable _shellModules;

	public int Order => 1000;

	public AreaApplicationModelProvider(ModuleTable shellModules)
	{
		_shellModules = shellModules;
	}

	public void OnProvidersExecuted(ApplicationModelProviderContext context)
	{
		foreach (ControllerModel item in context.Result.Controllers.Where((ControllerModel c) => c.ControllerType.GetCustomAttributes(typeof(NonAreaAttribute), inherit: true).Length == 0))
		{
			if (_shellModules.TryGetModule(item.ControllerType.Assembly, out var shellModule) && !shellModule.IsAppModule())
			{
				item.RouteValues["area"] = shellModule.Name;
			}
		}
	}

	public void OnProvidersExecuting(ApplicationModelProviderContext context)
	{
	}
}

using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Modules;
using EPiServer.Web;
using Microsoft.AspNetCore.Mvc.Razor;

namespace EPiServer.Shell.Web.Internal;

public class ShellModuleLocationExpander : IViewLocationExpander
{
	private readonly RazorViewEngineOptions _razorViewEngineOptions;

	public ShellModuleLocationExpander(RazorViewEngineOptions razorViewEngineOptions)
	{
		_razorViewEngineOptions = razorViewEngineOptions;
	}

	public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
	{
		object obj = context.ActionContext.RouteData.Values["module"];
		if (obj is ShellModule shellModule)
		{
			if (!string.IsNullOrEmpty(context.AreaName))
			{
				viewLocations = _razorViewEngineOptions.ViewLocationFormats.Concat(viewLocations).Distinct();
			}
			string controllerPrefix = shellModule.Manifest.Route.ControllerPrefix;
			if (!string.IsNullOrWhiteSpace(controllerPrefix) && context.ControllerName.ToLowerInvariant().StartsWith(controllerPrefix.ToLowerInvariant()))
			{
				viewLocations = (from v in viewLocations
					where v.Contains("{1}")
					select v.Replace("{1}", context.ControllerName.Substring(controllerPrefix.Length))).Concat(viewLocations).ToArray();
			}
			foreach (string viewLocation in viewLocations)
			{
				yield return UriUtil.Combine(ResolveViewFolderForModule(shellModule), viewLocation);
			}
			yield break;
		}
		foreach (string viewLocation2 in viewLocations)
		{
			yield return viewLocation2;
		}
	}

	public void PopulateValues(ViewLocationExpanderContext context)
	{
	}

	private string ResolveViewFolderForModule(ShellModule shellModule)
	{
		if (string.IsNullOrWhiteSpace(shellModule.Manifest.ViewFolder))
		{
			return shellModule.Name + ".Views";
		}
		return shellModule.Manifest.ViewFolder;
	}
}

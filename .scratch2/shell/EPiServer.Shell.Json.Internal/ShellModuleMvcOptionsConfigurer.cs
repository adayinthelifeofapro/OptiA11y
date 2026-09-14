using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Formatters;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace EPiServer.Shell.Json.Internal;

internal class ShellModuleMvcOptionsConfigurer(IOptions<MvcOptions> mvcOptions)
{
	/// <summary>
	///       Allows initialization with a set of modules
	///       </summary>
	/// <param name="uniqueModules">
	/// </param>
	public void Initialize(IEnumerable<ShellModule> uniqueModules)
	{
		foreach (ShellModule uniqueModule in uniqueModules)
		{
			RegisterAssembliesForInputConvention(uniqueModule.Assemblies);
		}
	}

	private void RegisterAssembliesForInputConvention(IEnumerable<Assembly> assemblies)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		foreach (Assembly assembly in assemblies)
		{
			if (mvcOptions.Value.Conventions.OfType<ApplyJsonFormatterConvention>().All((ApplyJsonFormatterConvention c) => c.Assembly != assembly))
			{
				mvcOptions.Value.Conventions.Add((IApplicationModelConvention)new ApplyJsonFormatterConvention(assembly));
			}
		}
	}
}

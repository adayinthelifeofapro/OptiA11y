using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules;
using EPiServer.Shell.UI;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Html helper for writing the dojo configuration to the html page
///       </summary>
public static class DojoConfigurationHelper
{
	/// <summary>
	///       Returns a script tag containing a dojoConfig object.
	///       </summary>
	public static IHtmlContent ConfigureDojo(DojoConfig dojoConfig)
	{
		return ConfigureDojo(dojoConfig, registerModulePaths: true);
	}

	/// <summary>
	///       Returns a script tag containing a dojoConfig object.
	///       </summary>
	/// <param name="parseOnLoad">indicate if dojo should parse the DOM on load</param>
	/// <param name="registerModulePaths">if set to <c>true</c> include configured module paths.</param>
	/// <param name="async">if set to <c>true</c> [async].</param>
	/// <returns>
	///       A string containing dojoConfig object. var dojoConfig =  { ... };
	///       </returns>
	public static IHtmlContent ConfigureDojo(bool parseOnLoad, bool registerModulePaths, bool async)
	{
		return ConfigureDojo(new DojoConfig
		{
			ParseOnLoad = parseOnLoad,
			Async = async,
			IoPublish = true
		}, registerModulePaths);
	}

	/// <summary>
	///       Returns a script tag containing a dojoConfig object.
	///       </summary>
	public static IHtmlContent ConfigureDojo(DojoConfig dojoConfig, bool registerModulePaths)
	{
		if (registerModulePaths)
		{
			RegisterModulePaths(dojoConfig);
		}
		TagBuilder tagBuilder = new TagBuilder("script");
		tagBuilder.Attributes["type"] = "text/javascript";
		tagBuilder.InnerHtml.AppendHtml(string.Format(CultureInfo.CurrentCulture, "var dojoConfig = {0};", dojoConfig.Serialize()));
		using StringWriter stringWriter = new StringWriter();
		tagBuilder.WriteTo(stringWriter, HtmlEncoder.Default);
		return new HtmlString(stringWriter.ToString());
	}

	private static void RegisterModulePaths(DojoConfig dojoConfig)
	{
		foreach (ShellModule module in ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).GetModules())
		{
			foreach (DojoPath dojoModule in module.Manifest.DojoModules)
			{
				Uri uri = new Uri(dojoModule.Path, UriKind.RelativeOrAbsolute);
				dojoConfig.Paths[dojoModule.Name] = (uri.IsAbsoluteUri ? dojoModule.Path : Paths.ToClientResource(module.Name, dojoModule.Path));
			}
			ShellModuleManifest manifest = module.Manifest;
			if (manifest == null || manifest.Dojo == null)
			{
				continue;
			}
			foreach (DojoPath path in module.Manifest.Dojo.Paths)
			{
				Uri uri2 = new Uri(path.Path, UriKind.RelativeOrAbsolute);
				dojoConfig.Paths[path.Name] = (uri2.IsAbsoluteUri ? path.Path : Paths.ToClientResource(module.Name, path.Path));
			}
			foreach (DojoAlias alias in manifest.Dojo.Aliases)
			{
				dojoConfig.Aliases.Add(new string[2] { alias.From, alias.To });
			}
			foreach (DojoPackage package in manifest.Dojo.Packages)
			{
				Uri uri3 = new Uri(package.Location, UriKind.RelativeOrAbsolute);
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{ "Name", package.Name },
					{
						"Location",
						uri3.IsAbsoluteUri ? package.Location : Paths.ToClientResource(module.Name, package.Location)
					}
				};
				if (package.Main != null)
				{
					dictionary.Add("Main", package.Main);
				}
				if (package.PackageMap != null)
				{
					DojoPackageMap dojoPackageMap = manifest.Dojo.PackageMaps.FirstOrDefault((DojoPackageMap m) => package.PackageMap.Equals(m.Name, StringComparison.Ordinal));
					if (dojoPackageMap != null)
					{
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						foreach (DojoPackageMapping mapping in dojoPackageMap.Mappings)
						{
							dictionary2.Add(mapping.Key, mapping.Value);
						}
						dictionary.Add("PackageMap", dictionary2);
					}
				}
				dojoConfig.Packages.Add(dictionary);
			}
		}
	}
}

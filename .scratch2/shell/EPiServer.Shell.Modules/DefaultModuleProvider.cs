using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Configuration;
using EPiServer.Web;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Provides shell module for the web application.
///       </summary>
[ModuleProvider]
internal class DefaultModuleProvider : IModuleProvider
{
	private readonly ILogger<DefaultModuleProvider> _log;

	private readonly IModuleFinder _finder;

	private readonly PublicModuleOptions _publicOptions;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ConfigModuleProvider" /> class.
	///       </summary>
	/// <param name="finder">The module finder.</param>
	/// <param name="options">
	///   <see cref="T:EPiServer.Shell.Modules.PublicModuleOptions" /> to read the module configuration</param>
	/// <param name="logger">The logger.</param>
	public DefaultModuleProvider(IModuleFinder finder, PublicModuleOptions options, ILogger<DefaultModuleProvider> logger)
	{
		ArgumentNullException.ThrowIfNull(finder, "finder");
		ArgumentNullException.ThrowIfNull(options, "options");
		_finder = finder;
		_publicOptions = options;
		_log = logger;
	}

	public IEnumerable<ShellModule> GetModules()
	{
		string routeBasePath = _publicOptions.RootPath.Trim(new char[2] { '~', '/' }) + "/";
		string moduleResourcePath = VirtualPathUtilityEx.ToAbsolute("~/");
		ShellModule moduleInDirectory = _finder.GetModuleInDirectory(routeBasePath, moduleResourcePath, Enumerable.Empty<string>(), AutoDiscoveryLevel.Minimal, "App");
		if (moduleInDirectory == null)
		{
			_log.FailedToGetModuleFromPath(routeBasePath);
			return Array.Empty<ShellModule>();
		}
		moduleInDirectory.ClientResourcePath = moduleInDirectory.Manifest.ClientResourceRelativePath ?? VirtualPathUtilityEx.ToAbsolute("~/ClientResources/");
		return new ShellModule[1] { moduleInDirectory };
	}
}

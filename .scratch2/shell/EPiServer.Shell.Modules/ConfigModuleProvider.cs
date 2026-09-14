using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules.Internal;
using EPiServer.Web;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Analyzes the configuration and initializes built-in and user-defined modules.
///       </summary>
/// <remarks>This class is used internally by EPiServer and should not be instantiated.</remarks>
[ModuleProvider]
public class ConfigModuleProvider : IModuleProvider
{
	private readonly ILogger<ConfigModuleProvider> _log;

	private readonly IModuleFinder _finder;

	private readonly PublicModuleOptions _publicOptions;

	private readonly ProtectedModuleOptions _protectedOptions;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ConfigModuleProvider" /> class.
	///       </summary>
	/// <param name="finder">
	/// </param>
	/// <param name="publicOptions">The PublicModuleOptions to read the configuration.</param>
	/// <param name="protectedOptions">The ProtectedModuleOptions to read the configuration.</param>
	/// <param name="logger">The logger instance.</param>
	public ConfigModuleProvider(IModuleFinder finder, PublicModuleOptions publicOptions, ProtectedModuleOptions protectedOptions, ILogger<ConfigModuleProvider> logger)
	{
		_publicOptions = publicOptions;
		_protectedOptions = protectedOptions;
		_finder = finder;
		_log = logger;
	}

	/// <summary>
	///       Initializes modules based on configuration
	///       </summary>
	public IEnumerable<ShellModule> GetModules()
	{
		List<ShellModule> configuredModules = GetConfiguredModules(_protectedOptions);
		foreach (ShellModule item in configuredModules)
		{
			if (string.IsNullOrWhiteSpace(item.AuthorizationPolicy))
			{
				item.Manifest.AuthorizationPolicy = "episerver:defaultshellmodule";
			}
			if (string.IsNullOrWhiteSpace(item.ClientAuthorizationPolicy))
			{
				item.Manifest.ClientAuthorizationPolicy = "episerver:defaultshellmodule";
			}
		}
		List<ShellModule> configuredModules2 = GetConfiguredModules(_publicOptions);
		foreach (ShellModule item2 in configuredModules2)
		{
			item2.IsPublic = true;
		}
		return configuredModules2.Union(configuredModules).ToList();
	}

	private List<ShellModule> GetConfiguredModules(ModuleOptionsBase moduleInfo)
	{
		string rootPath = VirtualPathUtilityEx.AppendTrailingSlash(moduleInfo.RootPath);
		ConcurrentBag<ShellModule> threadSafeSetOfModules = new ConcurrentBag<ShellModule>();
		Parallel.ForEach(moduleInfo.Items, new ParallelOptions
		{
			MaxDegreeOfParallelism = Environment.ProcessorCount
		}, delegate(ModuleDetails i)
		{
			string text = i.ResourcePath.Replace("{rootpath}", rootPath).Replace("{modulename}", i.Name);
			ShellModule shellModule = _finder.GetModuleInDirectory(rootPath, text, i.Assemblies, moduleInfo.AutoDiscovery, i.Name) ?? (text.StartsWith('~') ? _finder.GetModuleInDirectory(rootPath, text.Substring(1), i.Assemblies, moduleInfo.AutoDiscovery, i.Name) : null);
			if (shellModule != null)
			{
				string path = i.ClientResourcePath;
				if (!string.IsNullOrEmpty(path) && Paths.TryToAbsolute(ref path))
				{
					shellModule.ClientResourcePath = VirtualPathUtilityEx.AppendTrailingSlash(path);
				}
				threadSafeSetOfModules.Add(shellModule);
			}
		});
		List<ShellModule> list = threadSafeSetOfModules.ToList();
		if (moduleInfo.AutoDiscovery == AutoDiscoveryLevel.Modules)
		{
			_log.AutoDiscoveringModules();
			IList<ShellModule> modulesInSubdirectories = _finder.GetModulesInSubdirectories(rootPath, moduleInfo.AutoDiscovery);
			list.AddRange(modulesInSubdirectories.Except(list));
		}
		return list;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Extensions.Internal;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Keep tracks of module to assembly mappings
///       </summary>
public class ModuleTable : IEnumerable<ShellModule>, IEnumerable
{
	private class FallbackModule : ShellModule
	{
		public FallbackModule()
		{
			base.Controllers = Array.Empty<Type>();
			base.RouteBasePath = "/";
			base.ResourceBasePath = "/";
		}

		public override string GetHelpUrl()
		{
			return null;
		}
	}

	internal const string ApplicationModuleName = "App";

	private readonly ILogger<ModuleTable> _log;

	private readonly Dictionary<string, ShellModule> _moduleCache = new Dictionary<string, ShellModule>(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	///       Initializes a new instance of the ModuleTable class.
	///       </summary>
	/// <param name="log">The logger instance.</param>
	public ModuleTable(ILogger<ModuleTable> log)
	{
		_log = log;
	}

	/// <summary>
	///       Initializes the module manager for fist time usage.
	///       </summary>
	public virtual void Initialize(IEnumerable<ShellModule> modules)
	{
		_log.Initializing();
		foreach (ShellModule module in modules)
		{
			_log.AddingModule(module.Name);
			Add(module);
		}
	}

	/// <summary>
	///       Lists all module assemblies
	///       </summary>
	public virtual IList<Assembly> GetModuleAssemblies()
	{
		return _moduleCache.Values.SelectMany((ShellModule m) => m.Assemblies).ToList();
	}

	/// <summary>
	///       Gets managed shell modues.
	///       </summary>
	/// <returns>All modules</returns>
	public virtual IEnumerable<ShellModule> GetModules()
	{
		return _moduleCache.Values.Distinct();
	}

	/// <summary>
	///       Searches for a <see cref="T:EPiServer.Shell.Modules.ShellModule" /> with a specific namne.
	///       </summary>
	/// <param name="moduleName">Module name to search for</param>
	public virtual ShellModule FindModule(string moduleName)
	{
		if (_moduleCache.TryGetValue(moduleName, out var value))
		{
			return value;
		}
		return null;
	}

	/// <summary>
	///       Adds the specified module to the assembly-module mapping.
	///       </summary>
	/// <param name="newModule">The module that should be added.</param>
	public void Add(ShellModule newModule)
	{
		ArgumentNullException.ThrowIfNull(newModule, "newModule");
		ArgumentExceptionExtensions.ThrowIfPropertyIsNull(newModule.Name, null, null, "newModule.Name");
		if (_moduleCache.ContainsKey(newModule.Name))
		{
			_log.ReplacingExistingModule(newModule.Name);
		}
		else
		{
			_log.AddingModule(newModule.Name);
		}
		CheckAssembliesAlreadyRegistered(newModule);
		_moduleCache[newModule.Name] = newModule;
	}

	/// <summary>
	///       Adds a range of modules to the module table.
	///       </summary>
	/// <param name="modules">The modules to add.</param>
	public virtual void AddRange(IEnumerable<ShellModule> modules)
	{
		foreach (ShellModule module in modules)
		{
			Add(module);
		}
	}

	/// <summary>
	///       Tries to get the module associated to an assembly
	///       </summary>
	/// <param name="assembly">The associated assembly</param>
	/// <param name="shellModule">The module out parameter</param>
	/// <returns>True if the module exists</returns>
	public virtual bool TryGetModule(Assembly assembly, out ShellModule shellModule)
	{
		shellModule = _moduleCache.Values.FirstOrDefault((ShellModule m) => m.Assemblies.Contains(assembly));
		return shellModule != null;
	}

	/// <summary>
	///       Resolves a path to a resource relative to the specified module.
	///       </summary>
	/// <param name="moduleName">Name of the module.</param>
	/// <param name="moduleRelativePath">The module relative path.</param>
	/// <returns>An absolute path to a resource within a module.</returns>
	public virtual string ResolvePath(string moduleName, string moduleRelativePath)
	{
		if (TryAbsolutePath(ref moduleRelativePath))
		{
			return moduleRelativePath;
		}
		return Combine((FindModule(moduleName) ?? CreateFallbackModule()).ResourceBasePath, moduleRelativePath);
	}

	/// <summary>
	///       Resolves a path to a resource relative to the specified module.
	///       </summary>
	/// <param name="module">The Shell module.</param>
	/// <param name="moduleRelativePath">The module relative path.</param>
	/// <returns>
	///       An absolute path to a resource within a module.
	///       </returns>
	public static string ResolvePath(ShellModule module, string moduleRelativePath)
	{
		if (TryAbsolutePath(ref moduleRelativePath))
		{
			return moduleRelativePath;
		}
		if (module == null)
		{
			throw new ArgumentNullException("module", "Unable to resolve path when module is null.");
		}
		return Combine(module.ResourceBasePath, moduleRelativePath);
	}

	/// <summary>
	///       Resolves a path to a client resource relative to the specified module.
	///       </summary>
	/// <param name="moduleName">Name of the module.</param>
	/// <param name="moduleRelativePath">The module relative path.</param>
	/// <returns>An absolute path to a resource within a module.</returns>
	public virtual string ResolveClientPath(string moduleName, string moduleRelativePath)
	{
		if (TryAbsolutePath(ref moduleRelativePath))
		{
			return moduleRelativePath;
		}
		string path = Combine((FindModule(moduleName) ?? CreateFallbackModule()).ClientResourcePath, moduleRelativePath);
		Paths.TryToAbsolute(ref path);
		return path;
	}

	/// <summary>
	///       Resolves a path to a client resource relative to the specified module.
	///       </summary>
	/// <param name="module">Shell module.</param>
	/// <param name="moduleRelativePath">The module relative path.</param>
	/// <returns>
	///       An absolute path to a resource within a module.
	///       </returns>
	public static string ResolveClientPath(ShellModule module, string moduleRelativePath)
	{
		if (TryAbsolutePath(ref moduleRelativePath))
		{
			return moduleRelativePath;
		}
		if (module == null)
		{
			throw new ArgumentNullException("module", "Unable to resolve path when module is null.");
		}
		return Combine(module.ClientResourcePath, moduleRelativePath);
	}

	internal static bool TryAbsolutePath(ref string moduleRelativePath)
	{
		if (!string.IsNullOrEmpty(moduleRelativePath))
		{
			char c = moduleRelativePath[0];
			if ((c == '/' || c == '~') ? true : false)
			{
				if (!Paths.TryToAbsolute(ref moduleRelativePath))
				{
					moduleRelativePath = moduleRelativePath.TrimStart('~');
				}
				return true;
			}
		}
		return false;
	}

	/// <summary>
	///       Gets the shell module, or defaults to the application root.
	///       </summary>
	/// <param name="assembly">The assembly within the requested module.</param>
	/// <returns>A shell modeule, or a default one.</returns>
	public virtual ShellModule GetModuleOrDefault(Assembly assembly)
	{
		if (TryGetModule(assembly, out var shellModule))
		{
			return shellModule;
		}
		return CreateFallbackModule();
	}

	private ShellModule CreateFallbackModule()
	{
		if (_moduleCache.TryGetValue("App", out var value))
		{
			return value;
		}
		return new FallbackModule
		{
			Name = "App"
		};
	}

	/// <summary>
	///       Combine two HTTP-scheme URLs or paths. Use for simple and robust concatentation of slash-separated paths,
	///       even if part of a full http: URL.
	///       </summary>
	/// <param name="part1">First part, may include scheme, host etc</param>
	/// <param name="part2">Second part, may include traling query string, fragment etc</param>
	/// <returns>The correctly combined paths regardless of if part1 ends or not, or part2 starts or not, with "/"</returns>
	internal static string Combine(string part1, string part2)
	{
		if (string.IsNullOrEmpty(part1) || string.IsNullOrEmpty(part2))
		{
			return part1 + part2;
		}
		bool flag = part1[part1.Length - 1] == '/';
		bool flag2 = part2[0] == '/';
		if (flag ^ flag2)
		{
			return part1 + part2;
		}
		if (flag & flag2)
		{
			return part1 + part2.Substring(1);
		}
		return part1 + "/" + part2;
	}

	private void CheckAssembliesAlreadyRegistered(ShellModule module)
	{
		HashSet<Assembly> hashSet = new HashSet<Assembly>(module.Assemblies);
		foreach (KeyValuePair<string, ShellModule> item in _moduleCache)
		{
			if (hashSet.Overlaps(item.Value.Assemblies))
			{
				hashSet.IntersectWith(item.Value.Assemblies);
			}
		}
	}

	/// <summary>
	///       Gets a <see cref="T:EPiServer.Shell.Modules.ShellModule" /> enumerator.
	///       </summary>
	/// <returns>An enumerator for <see cref="T:EPiServer.Shell.Modules.ShellModule" /> instances in the module table.</returns>
	public virtual IEnumerator<ShellModule> GetEnumerator()
	{
		return _moduleCache.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

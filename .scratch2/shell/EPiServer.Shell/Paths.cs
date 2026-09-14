using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using EPiServer.Web;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell;

/// <summary>
///       Helps resolve path's to resources in EPiServer Shell.
///       </summary>
public static class Paths
{
	/// <summary>
	///       The name of the shell framework module. This constant is used internally.
	///       </summary>
	public const string ShellModuleName = "Shell";

	[CompilerGenerated]
	private static string _003CProtectedRootPath_003Ek__BackingField;

	[CompilerGenerated]
	private static string _003CPublicRootPath_003Ek__BackingField;

	/// <summary>
	///       The root path of protected modules
	///       </summary>
	public static string ProtectedRootPath
	{
		get
		{
			if (_003CProtectedRootPath_003Ek__BackingField == null)
			{
				string path = ServiceProviderExtensions.GetInstance<ProtectedModuleOptions>(ServiceLocator.Current).RootPath;
				bool num = TryToAbsolute(ref path);
				path = VirtualPathUtilityEx.AppendTrailingSlash(path);
				if (num)
				{
					_003CProtectedRootPath_003Ek__BackingField = path;
				}
				return path;
			}
			return _003CProtectedRootPath_003Ek__BackingField;
		}
	}

	/// <summary>
	///       The root path of public modules
	///       </summary>
	public static string PublicRootPath
	{
		get
		{
			if (_003CPublicRootPath_003Ek__BackingField == null)
			{
				string path = ServiceProviderExtensions.GetInstance<PublicModuleOptions>(ServiceLocator.Current).RootPath;
				bool num = TryToAbsolute(ref path);
				path = VirtualPathUtilityEx.AppendTrailingSlash(path);
				if (num)
				{
					_003CPublicRootPath_003Ek__BackingField = path;
				}
				return path;
			}
			return _003CPublicRootPath_003Ek__BackingField;
		}
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a master page or config file
	///       </summary>
	/// <param name="shellModuleRelativeResourcePath">A path relative to the shell framework module's root path, e.g. "ClientResources/packages.config"</param>
	/// <returns>
	/// </returns>
	public static string ToShellResource(string shellModuleRelativeResourcePath)
	{
		return ToResource("Shell", shellModuleRelativeResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework client resource such as a javascript file or css
	///       </summary>
	/// <param name="shellModuleRelativeClientResourcePath">A path relative to the shell framework module's root path, e.g. "ClientResources/EPiJQuery.js"</param>
	/// <returns>
	/// </returns>
	public static string ToShellClientResource(string shellModuleRelativeClientResourcePath)
	{
		return ToClientResource("Shell", shellModuleRelativeClientResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a master page or config file
	///       </summary>
	/// <param name="moduleName">Name of the module.</param>
	/// <param name="moduleRelativeResourcePath">A path relative to the given module's root path, e.g. "ClientResources/packages.config"</param>
	/// <returns>A resolved path.</returns>
	/// <remarks>If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.</remarks>
	public static string ToResource(string moduleName, string moduleRelativeResourcePath)
	{
		return ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).ResolvePath(moduleName, moduleRelativeResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a master page or config file
	///       </summary>
	/// <param name="moduleAssembly">The module assembly.</param>
	/// <param name="moduleRelativeResourcePath">A path relative to the given module's root path, e.g. "ClientResources/packages.config"</param>
	/// <returns>
	///       A resolved path.
	///       </returns>
	/// <remarks>
	///       If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.
	///       </remarks>
	/// <exception cref="T:System.ArgumentException">Shell module cannot be found by moduleAssembly.</exception>
	/// <exception cref="T:System.ArgumentNullException">Exception is thrown when moduleAssembly is null.</exception>
	public static string ToResource(Assembly moduleAssembly, string moduleRelativeResourcePath)
	{
		if (moduleAssembly == null)
		{
			throw new ArgumentNullException("moduleAssembly", "Unable to resolve path to module resources folder by null assembly.");
		}
		if (!ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).TryGetModule(moduleAssembly, out var shellModule))
		{
			throw new ArgumentException("Unable to find a module by assembly '" + moduleAssembly?.ToString() + "'", "moduleAssembly");
		}
		return ModuleTable.ResolvePath(shellModule, moduleRelativeResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a master page or config file
	///       </summary>
	/// <param name="typeInModuleAssembly">Type contained in module assembly.</param>
	/// <param name="moduleRelativeResourcePath">A path relative to the given module's root path, e.g. "ClientResources/packages.config"</param>
	/// <returns>
	///       A resolved path.
	///       </returns>
	/// <exception cref="T:System.ArgumentException">Shell module cannot be found by moduleAssembly.</exception>
	/// <exception cref="T:System.ArgumentNullException">Exception is thrown when specified typeInModuleAssembly is null.</exception>
	/// <remarks>
	///       If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.
	///       </remarks>
	public static string ToResource(Type typeInModuleAssembly, string moduleRelativeResourcePath)
	{
		if (typeInModuleAssembly == null)
		{
			throw new ArgumentNullException("typeInModuleAssembly", "Type from module assembly must be not null to resolve path in module folder.");
		}
		return ToResource(typeInModuleAssembly.Assembly, moduleRelativeResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a javascript file or css
	///       </summary>
	/// <param name="moduleName">Name of the module.</param>
	/// <param name="moduleRelativeClientResourcePath">A path relative to the given module's root path, e.g. "Content/MyStyle.css"</param>
	/// <returns>A resolved path.</returns>
	/// <remarks>If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.</remarks>
	public static string ToClientResource(string moduleName, string moduleRelativeClientResourcePath)
	{
		return ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).ResolveClientPath(moduleName, moduleRelativeClientResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a javascript file or css
	///       </summary>
	/// <param name="moduleAssembly">The module assembly.</param>
	/// <param name="moduleRelativeClientResourcePath">A path relative to the given module's root path, e.g. "Content/MyStyle.css"</param>
	/// <returns>
	///       A resolved path.
	///       </returns>
	/// <remarks>
	///       If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.
	///       </remarks>
	/// <exception cref="T:System.ArgumentException">Shell module cannot be found by moduleAssembly.</exception>
	/// <exception cref="T:System.ArgumentNullException">Exception is thrown when moduleAssembly is null.</exception>
	public static string ToClientResource(Assembly moduleAssembly, string moduleRelativeClientResourcePath)
	{
		if (moduleAssembly == null)
		{
			throw new ArgumentNullException("moduleAssembly", "Unable to resolve path to module client resources folder by null assembly.");
		}
		if (!ServiceProviderExtensions.GetInstance<ModuleTable>(ServiceLocator.Current).TryGetModule(moduleAssembly, out var shellModule))
		{
			throw new ArgumentException("Did not find a module by assembly '" + moduleAssembly?.ToString() + "'", "moduleAssembly");
		}
		return ModuleTable.ResolveClientPath(shellModule, moduleRelativeClientResourcePath);
	}

	/// <summary>
	///       Resolves the path of EPiServer Framework module resource such as a javascript file or css
	///       </summary>
	/// <param name="typeInModuleAssembly">The type in module assembly.</param>
	/// <param name="moduleRelativeClientResourcePath">A path relative to the given module's root path, e.g. "Content/MyStyle.css"</param>
	/// <returns>
	///       A resolved path.
	///       </returns>
	/// <exception cref="T:System.ArgumentException">Shell module cannot be found by moduleAssembly.</exception>
	/// <exception cref="T:System.ArgumentNullException">Exception is thrown when typeInModuleAssembly is null.</exception>
	/// <remarks>
	///       If moduleRelativePath is an absolute or app-relative url it will be returned without beeing combined to the module's resource path.
	///       </remarks>
	public static string ToClientResource(Type typeInModuleAssembly, string moduleRelativeClientResourcePath)
	{
		if (typeInModuleAssembly == null)
		{
			throw new ArgumentNullException("typeInModuleAssembly", "Type from module assembly must be not null to resolve path in module folder.");
		}
		return ToClientResource(typeInModuleAssembly.Assembly, moduleRelativeClientResourcePath);
	}

	/// <summary>
	///       Tries to absolute an url with some error handling that should help in unit test scenarios.
	///       </summary>
	/// <param name="path">The path.</param>
	/// <returns>True if the path was modified.</returns>
	internal static bool TryToAbsolute(ref string path)
	{
		try
		{
			path = VirtualPathUtilityEx.ToAbsolute(path);
			return true;
		}
		catch (ArgumentException exception)
		{
			ServiceProviderExtensions.GetInstance<ILogger<ShellModule>>(ServiceLocator.Current).FailedToMakePathAbsolute(exception, path ?? "null");
		}
		return false;
	}
}

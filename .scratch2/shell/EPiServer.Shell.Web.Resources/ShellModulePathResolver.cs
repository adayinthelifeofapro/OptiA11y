using System.Reflection;
using EPiServer.Framework.Modules;
using EPiServer.Shell.Modules;

namespace EPiServer.Shell.Web.Resources;

internal class ShellModulePathResolver : IModuleResourceResolver
{
	private readonly ModuleTable _moduleTable;

	public string ProtectedRootPath => Paths.ProtectedRootPath;

	public ShellModulePathResolver(ModuleTable moduleTable)
	{
		_moduleTable = moduleTable;
	}

	public string ResolveClientPath(string moduleName, string moduleRelativePath)
	{
		return Paths.ToClientResource(moduleName, moduleRelativePath);
	}

	public string ResolvePath(string moduleName, string moduleRelativePath)
	{
		return Paths.ToResource(moduleName, moduleRelativePath);
	}

	public bool TryResolveClientPath(Assembly assembly, string relativePath, out string resolvedPath)
	{
		resolvedPath = null;
		if (!_moduleTable.TryGetModule(assembly, out var shellModule))
		{
			return false;
		}
		resolvedPath = ModuleTable.ResolveClientPath(shellModule, relativePath);
		return true;
	}

	public bool TryResolvePath(Assembly assembly, string relativePath, out string resolvedPath)
	{
		resolvedPath = null;
		if (!_moduleTable.TryGetModule(assembly, out var shellModule))
		{
			return false;
		}
		resolvedPath = ModuleTable.ResolvePath(shellModule, relativePath);
		return true;
	}
}

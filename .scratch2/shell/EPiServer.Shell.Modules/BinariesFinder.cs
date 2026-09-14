using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using EPiServer.Web;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Locates modules assemblies
///       NuGet compatible approach is used to find modules binaries: http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package.
///       </summary>
/// <remarks>
///       Possible structure of module binaries folder:
///       bin\module.dll
///       bin\NET20\module.dll
///       bin\NET40\module.dll
///       clientbin\Silverlight4\module.dll
///       clientbin\sl4\module.dll
///       </remarks>
public class BinariesFinder
{
	private const string BinariesSubfolder = "bin";

	private readonly IFileProvider _fileProvider;

	private readonly Version _runtimeVersion;

	private readonly ILogger<BinariesFinder> _log;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.BinariesFinder" /> class.
	///       </summary>
	/// <param name="virtualPathProvider">The virtual path provider.</param>
	/// <param name="logger">The logger instance.</param>
	public BinariesFinder(IFileProvider virtualPathProvider, ILogger<BinariesFinder> logger)
	{
		_fileProvider = virtualPathProvider;
		_runtimeVersion = VersionUtility.DefaultTargetFrameworkVersion;
		_log = logger;
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.BinariesFinder" /> class.
	///       </summary>
	/// <param name="virtualPathProvider">The virtual path provider.</param>
	/// <param name="runtimeVersion">The runtime version.</param>
	/// <param name="logger">The logger instance.</param>
	public BinariesFinder(IFileProvider virtualPathProvider, Version runtimeVersion, ILogger<BinariesFinder> logger)
	{
		_fileProvider = virtualPathProvider;
		_runtimeVersion = runtimeVersion;
		_log = logger;
	}

	/// <summary>
	///       Gets all binaries in modules.
	///       </summary>
	/// <param name="modulesRootPath">
	/// </param>
	/// <remarks>
	///       If module supports several runtime versions method returns binaries related to the current runtime version
	///       </remarks>
	/// <returns>
	/// </returns>
	public IList<IFileInfo> GetAllBinaries(string modulesRootPath)
	{
		return GetAllBinariesInternal(GetBinariesForModule, modulesRootPath);
	}

	/// <summary>
	///       Gets all silverlight binaries in modules.
	///       </summary>
	/// <param name="modulesRootPath">
	/// </param>
	/// <remarks>
	///       If module supports several runtime versions method returns binaries related to the current runtime version
	///       </remarks>
	/// <returns>
	/// </returns>
	public IList<IFileInfo> GetAllSilverlightBinaries(string modulesRootPath)
	{
		return GetAllBinariesInternal(GetSilverlightBinariesForModule, modulesRootPath);
	}

	/// <summary>
	///       Gets binaries for specified module path.
	///       </summary>
	/// <remarks>
	///       If module supports several runtime versions method returns binaries related to the current runtime version
	///       </remarks>
	/// <param name="moduleFolder">The module folder.</param>
	/// <returns>
	/// </returns>
	public IList<IFileInfo> GetBinariesForModule(string moduleFolder)
	{
		return GetBinariesForModuleInternal(moduleFolder, new FrameworkName(".NETFramework", _runtimeVersion));
	}

	/// <summary>
	///       Gets Silverlight binaries for specified module path.
	///       </summary>
	/// <remarks>
	///       If module supports several runtime versions method returns binaries related to the current runtime version
	///       </remarks>
	/// <param name="moduleFolder">The module folder.</param>
	/// <returns>
	/// </returns>
	public IList<IFileInfo> GetSilverlightBinariesForModule(string moduleFolder)
	{
		return GetBinariesForModuleInternal(moduleFolder, new FrameworkName("Silverlight", _runtimeVersion));
	}

	private List<IFileInfo> GetAllBinariesInternal(Func<string, IList<IFileInfo>> getModulesBinariesFunction, string modulesRootPath)
	{
		IDirectoryContents directoryContents = _fileProvider.GetDirectoryContents(modulesRootPath);
		if (directoryContents == null || !directoryContents.Exists)
		{
			_log.ModulesRootPathNotFound(modulesRootPath);
			return new List<IFileInfo>();
		}
		IEnumerable<IFileInfo> enumerable = from f in _fileProvider.GetDirectoryContents(modulesRootPath)
			where f.IsDirectory
			select f;
		List<IFileInfo> list = new List<IFileInfo>();
		foreach (IFileInfo item in enumerable)
		{
			IList<IFileInfo> collection = getModulesBinariesFunction(UriUtil.Combine(modulesRootPath, item.Name));
			list.AddRange(collection);
		}
		return list;
	}

	/// <summary>
	///       Gets binaries for the module.
	///       </summary>
	/// <param name="moduleFolder">The module folder.</param>
	/// <param name="targetFramework">Target framework to search assembly for</param>
	/// <returns>
	/// </returns>
	private List<IFileInfo> GetBinariesForModuleInternal(string moduleFolder, FrameworkName targetFramework)
	{
		IDirectoryContents directoryContents = _fileProvider.GetDirectoryContents(moduleFolder);
		if (directoryContents == null || !directoryContents.Exists)
		{
			_log.ModuleFolderDoesNotExist(moduleFolder);
			return new List<IFileInfo>();
		}
		IFileInfo fileInfo = (from f in _fileProvider.GetDirectoryContents(moduleFolder)
			where f.IsDirectory
			select f).FirstOrDefault((IFileInfo d) => d.Name != null && d.Name.Equals("bin", StringComparison.OrdinalIgnoreCase));
		if (fileInfo == null)
		{
			_log.BinariesFolderDoesNotExist("bin", moduleFolder);
			return new List<IFileInfo>();
		}
		IEnumerable<ModuleAssembly> moduleAssemblies = GetModuleAssemblies(UriUtil.Combine(moduleFolder, fileInfo.Name));
		VersionUtility.TryGetCompatibleItems(targetFramework, moduleAssemblies, out var compatibleItems);
		if (compatibleItems == null)
		{
			return new List<IFileInfo>();
		}
		return compatibleItems.Select((ModuleAssembly a) => a.AssemblyFile).ToList();
	}

	private IEnumerable<ModuleAssembly> GetModuleAssemblies(string binDir)
	{
		List<ModuleAssembly> list = (from f in GetBinariesFromDirectory(binDir)
			select new ModuleAssembly(f, null)).ToList();
		foreach (IFileInfo item in from f in _fileProvider.GetDirectoryContents(binDir)
			where f.IsDirectory
			select f)
		{
			FrameworkName frameworkName = VersionUtility.ParseFrameworkName(item.Name);
			list.AddRange(frameworkName.Identifier.Equals("Unsupported", StringComparison.OrdinalIgnoreCase) ? GetAssembliesFromFolderRecursive(UriUtil.Combine(binDir, item.Name), null) : GetAssembliesFromFolderRecursive(UriUtil.Combine(binDir, item.Name), frameworkName));
		}
		return list.Distinct();
	}

	private List<ModuleAssembly> GetAssembliesFromFolderRecursive(string dir, FrameworkName frameworkName)
	{
		List<ModuleAssembly> list = (from f in GetBinariesFromDirectory(dir)
			select new ModuleAssembly(f, frameworkName)).ToList();
		foreach (IFileInfo item in from d in _fileProvider.GetDirectoryContents(dir)
			where d.IsDirectory
			select d)
		{
			list.AddRange(GetAssembliesFromFolderRecursive(UriUtil.Combine(dir, item.Name), frameworkName));
		}
		return list;
	}

	private IEnumerable<IFileInfo> GetBinariesFromDirectory(string binDir)
	{
		return from f in _fileProvider.GetDirectoryContents(binDir)
			where !f.IsDirectory
			where (Path.GetExtension(f.Name) ?? string.Empty).Equals(".dll", StringComparison.OrdinalIgnoreCase)
			select f;
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.TypeScanner;
using EPiServer.Shell.Configuration;
using EPiServer.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <inheritdoc />
public class ModuleFinder : IModuleFinder
{
	/// <summary>
	///       By default a folder named "_Protected" is always ignored by the module finder
	///       </summary>
	public const string ProtectedModulesFolderName = "_protected";

	private readonly BinariesFinder _binariesFinder;

	/// <summary>
	///       Assemblies loaded to current application domain
	///       </summary>
	private Assembly[] _loadedAssemblies;

	private readonly IServiceProvider _serviceProvider;

	private readonly ILogger<ModuleFinder> _log;

	private IFileProvider FileProvider { get; }

	private string RootPath => "/";

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleFinder" /> class.
	///       </summary>
	public ModuleFinder(IFileProvider fileProvider, IServiceProvider serviceProvider, ILogger<ModuleFinder> logger, ILogger<BinariesFinder> binaryiesFinderLogger)
	{
		FileProvider = fileProvider;
		_serviceProvider = serviceProvider;
		_log = logger;
		_binariesFinder = new BinariesFinder(fileProvider, binaryiesFinderLogger);
	}

	/// <inheritdoc />
	public virtual IList<ShellModule> GetModulesInSubdirectories(string rootPath, AutoDiscoveryLevel discoveryMode)
	{
		_log.AnalyzingPathForModules(rootPath);
		ConcurrentBag<ShellModule> concurrentBag = new ConcurrentBag<ShellModule>();
		if (FileProvider == null)
		{
			_log.NotAnalyzingDirectoriesNullProvider();
			return new List<ShellModule>();
		}
		IDirectoryContents directoryContents = FileProvider.GetDirectoryContents(rootPath);
		if (directoryContents == null)
		{
			return new List<ShellModule>();
		}
		foreach (IFileInfo item in directoryContents)
		{
			if (string.Equals(item.Name, "_protected", StringComparison.OrdinalIgnoreCase))
			{
				_log.IgnoringProtectedFolder("_protected");
				continue;
			}
			ShellModule moduleInDirectory = GetModuleInDirectory(rootPath, Path.Combine(rootPath, item.Name), Array.Empty<string>(), discoveryMode, null);
			if (moduleInDirectory != null)
			{
				concurrentBag.Add(moduleInDirectory);
			}
		}
		return concurrentBag.ToList();
	}

	/// <inheritdoc />
	public virtual ShellModule GetModuleInDirectory(string routeBasePath, string moduleResourcePath, IEnumerable<string> configuredAssemblyNames, AutoDiscoveryLevel discoveryMode, string configuredName)
	{
		_log.LookingForModuleInPath(moduleResourcePath);
		if (FileProvider == null)
		{
			_log.NotAnalyzingModulePathNullProvider(moduleResourcePath);
			return null;
		}
		if (_loadedAssemblies == null)
		{
			_loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
		}
		string text = VirtualPathUtilityEx.AppendTrailingSlash(moduleResourcePath);
		IDirectoryContents directoryContents = FileProvider.GetDirectoryContents(text);
		if (directoryContents == null || !directoryContents.Exists)
		{
			_log.ModuleDirectoryNotFound(moduleResourcePath);
			return null;
		}
		ShellModuleManifest shellModuleManifest = ReadManifest(UriUtil.Combine(text, "module.config"));
		string routeBasePath2 = (shellModuleManifest.RouteBasePath ?? routeBasePath).TrimStart(new char[2] { '~', '/' });
		string name = ((!string.IsNullOrEmpty(configuredName)) ? configuredName : new DirectoryInfo(FileProvider.GetFileInfo(text).PhysicalPath).Name);
		Type instanceType = (string.IsNullOrEmpty(shellModuleManifest.Type) ? typeof(ShellModule) : Type.GetType(shellModuleManifest.Type));
		ShellModule shellModule = (ShellModule)ActivatorUtilities.CreateInstance(_serviceProvider, instanceType);
		shellModule.Name = name;
		shellModule.RouteBasePath = routeBasePath2;
		shellModule.ResourceBasePath = VirtualPathUtilityEx.ToAbsolute(text);
		shellModule.FileProvider = FileProvider;
		shellModule.TypeScannerLookup = _serviceProvider.GetRequiredService<ITypeScannerLookup>();
		shellModule.Manifest = shellModuleManifest;
		shellModule.Manifest.EnsureDefaultRoute(string.IsNullOrEmpty(shellModule.Name) ? "App" : shellModule.Name);
		shellModule.Assemblies = LoadAssemblies(configuredAssemblyNames, text, shellModule.Manifest, discoveryMode);
		if (!string.IsNullOrEmpty(shellModuleManifest.ClientResourceRelativePath))
		{
			string path = UriUtil.Combine(shellModule.ResourceBasePath, shellModuleManifest.ClientResourceRelativePath);
			if (Paths.TryToAbsolute(ref path))
			{
				shellModule.ClientResourcePath = VirtualPathUtilityEx.AppendTrailingSlash(path);
			}
		}
		_log.FoundModule(shellModule);
		return shellModule;
	}

	private List<Assembly> LoadAssemblies(IEnumerable<string> configuredAssemblyNames, string moduleDirectory, ShellModuleManifest manifest, AutoDiscoveryLevel discoveryMode)
	{
		List<Assembly> moduleAssemblies = new List<Assembly>();
		foreach (string configuredAssemblyName in configuredAssemblyNames)
		{
			_log.CheckingAssemblyConfiguredInWebConfig(configuredAssemblyName, moduleDirectory);
			Assembly assembly = Assembly.Load(configuredAssemblyName);
			RegisterModuleAssembly(assembly, ref moduleAssemblies);
		}
		foreach (AssemblyElement assembly4 in manifest.Assemblies)
		{
			_log.CheckingAssemblyConfiguredInManifest(assembly4.Assembly, moduleDirectory);
			Assembly assembly2 = Assembly.Load(assembly4.Assembly);
			RegisterModuleAssembly(assembly2, ref moduleAssemblies);
		}
		if (discoveryMode == AutoDiscoveryLevel.Modules && !moduleDirectory.Equals(RootPath, StringComparison.OrdinalIgnoreCase) && manifest.LoadFromBin)
		{
			_log.LookingForAssembliesInModuleDirectory(moduleDirectory);
			foreach (IFileInfo item in _binariesFinder.GetBinariesForModule(moduleDirectory))
			{
				_log.LoadingModulesAssembly(item.Name);
				string assemblyName = Path.GetFileNameWithoutExtension(item.Name);
				Assembly assembly3 = null;
				if (string.IsNullOrEmpty(assemblyName))
				{
					continue;
				}
				assembly3 = _loadedAssemblies.FirstOrDefault((Assembly assembly4) => string.Equals(assembly4.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
				if (assembly3 == null)
				{
					_log.TryToLoadAssemblyByName(assemblyName);
					if (!TryLoadAssemblyByName(assemblyName, out assembly3))
					{
						_log.ReadingAssemblyFromBinariesDirectory(item.ToString());
						byte[] array = ReadFileToEnd(item);
						_log.LoadingBytesOfAssembly(array.Length, item.Name);
						assembly3 = Assembly.Load(array);
					}
				}
				if (assembly3 != null)
				{
					RegisterModuleAssembly(assembly3, ref moduleAssemblies);
				}
			}
		}
		return moduleAssemblies;
	}

	/// <summary>
	///       Registers the specified assembly for module, if it was not listed in module assemblies.
	///       </summary>
	private bool RegisterModuleAssembly(Assembly assembly, ref List<Assembly> moduleAssemblies)
	{
		ArgumentNullException.ThrowIfNull(assembly, "assembly");
		if (moduleAssemblies.Any((Assembly a) => a.FullName.Equals(assembly.FullName, StringComparison.OrdinalIgnoreCase)))
		{
			_log.AssemblyAlreadyRegistered(assembly.FullName);
			return false;
		}
		_log.RegisteringAssembly(assembly.FullName);
		moduleAssemblies.Add(assembly);
		return true;
	}

	private bool TryLoadAssemblyByName(string assemblyName, out Assembly loadedAssembly)
	{
		try
		{
			loadedAssembly = Assembly.Load(assemblyName);
			return true;
		}
		catch (ArgumentException e)
		{
			logException(e);
		}
		catch (FileNotFoundException e2)
		{
			logException(e2);
		}
		catch (FileLoadException e3)
		{
			logException(e3);
		}
		catch (BadImageFormatException e4)
		{
			logException(e4);
		}
		loadedAssembly = null;
		return false;
		void logException(Exception exception)
		{
			_log.UnableToLoadModuleAssemblyByName(exception);
		}
	}

	private ShellModuleManifest ReadManifest(string manifestVirtualPath)
	{
		if (FileProvider.GetFileInfo(manifestVirtualPath).Exists)
		{
			try
			{
				IFileInfo fileInfo = FileProvider.GetFileInfo(manifestVirtualPath);
				_log.ReadingManifest(manifestVirtualPath);
				using MemoryStream memoryStream = new MemoryStream();
				using (Stream stream = fileInfo.CreateReadStream())
				{
					stream.CopyTo(memoryStream);
					memoryStream.Seek(0L, SeekOrigin.Begin);
				}
				ShellModuleManifest shellModuleManifest = ReadManifest(memoryStream);
				foreach (AssemblyElement assembly in shellModuleManifest.Assemblies)
				{
					if (string.IsNullOrEmpty(assembly.Assembly) && !string.IsNullOrEmpty(assembly.Name))
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The xml schema for the file '{0}' has changed since EPiServer CMS 6 Release Candidate and adding assemblies with <add name=\"{1}\"/> is no longer supported. Update your configuration to use this: <add assembly=\"{1}\"/>", manifestVirtualPath, assembly.Name));
					}
				}
				return shellModuleManifest;
			}
			catch (DirectoryNotFoundException)
			{
				_log.NoModuleManifestUsingDefaults(manifestVirtualPath);
			}
			catch (Exception ex2)
			{
				throw new MissingConfigurationException("Failed parsing module manifest located at '" + manifestVirtualPath + "' with error: " + ex2.Message, ex2);
			}
		}
		_log.NoModuleManifestUsingDefault(manifestVirtualPath);
		return new ShellModuleManifest();
	}

	protected virtual ShellModuleManifest ReadManifest(Stream manifestStream)
	{
		return ShellModuleManifest.Deserialize(manifestStream);
	}

	private static byte[] ReadFileToEnd(IFileInfo dll)
	{
		using Stream stream = dll.CreateReadStream();
		byte[] array = new byte[stream.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)stream.ReadByte();
		}
		return array;
	}
}

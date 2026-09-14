using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class ModuleFinderExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __AnalyzingPathForModulesCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2086194285, "AnalyzingPathForModules"), "Analyzing '{path}' for modules", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __NotAnalyzingDirectoriesNullProviderCallback = LoggerMessage.Define(LogLevel.Warning, new EventId(1156418680, "NotAnalyzingDirectoriesNullProvider"), "Not analyzing directories in modules due to VirtualPathProvider being null", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __IgnoringProtectedFolderCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(464699746, "IgnoringProtectedFolder"), "Ignoring '{folderName}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __LookingForModuleInPathCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(94173105, "LookingForModuleInPath"), "Looking for module in '{modulePath}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __NotAnalyzingModulePathNullProviderCallback = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(2016851302, "NotAnalyzingModulePathNullProvider"), "Not analyzing '{modulePath}' in modules due to VirtualPathProvider being null", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ModuleDirectoryNotFoundCallback = LoggerMessage.Define<string>(LogLevel.Error, new EventId(871708965, "ModuleDirectoryNotFound"), "The modules finder couldn't find a directory at '{modulePath}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, object, Exception?> __FoundModuleCallback = LoggerMessage.Define<object>(LogLevel.Debug, new EventId(1087076779, "FoundModule"), "Found module: {module}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __CheckingAssemblyConfiguredInWebConfigCallback = LoggerMessage.Define<string, string>(LogLevel.Debug, new EventId(1517539606, "CheckingAssemblyConfiguredInWebConfig"), "Checking assembly '{assemblyName}' configured in web.config for module in directory '{moduleDir}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __CheckingAssemblyConfiguredInManifestCallback = LoggerMessage.Define<string, string>(LogLevel.Debug, new EventId(1632801611, "CheckingAssemblyConfiguredInManifest"), "Checking assembly '{assemblyName}' configured in manifest from module in directory '{moduleDir}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __LookingForAssembliesInModuleDirectoryCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(707099425, "LookingForAssembliesInModuleDirectory"), "Looking for assemblies in module directory {moduleDir}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __LoadingModulesAssemblyCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1260730988, "LoadingModulesAssembly"), "Loading modules assembly {assemblyName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __TryToLoadAssemblyByNameCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(2115292175, "TryToLoadAssemblyByName"), "Try to load assembly by name '{assemblyName}'.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ReadingAssemblyFromBinariesDirectoryCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1329747605, "ReadingAssemblyFromBinariesDirectory"), "Reading assembly from binaries directory: {dllPath}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, int, string, Exception?> __LoadingBytesOfAssemblyCallback = LoggerMessage.Define<int, string>(LogLevel.Debug, new EventId(863533471, "LoadingBytesOfAssembly"), "Loading '{bytesCount}' bytes of assembly from library '{assemblyName}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __AssemblyAlreadyRegisteredCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(98573237, "AssemblyAlreadyRegistered"), "Assembly {assemblyFullName} is already registered for module, skipping it.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __RegisteringAssemblyCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(315155220, "RegisteringAssembly"), "Registering assembly {assemblyFullName} for module.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __UnableToLoadModuleAssemblyByNameCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(2015534397, "UnableToLoadModuleAssemblyByName"), "Unable to load module assembly by name.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ReadingManifestCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(245447534, "ReadingManifest"), "Reading manifest from: {manifestPath}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __NoModuleManifestUsingDefaultsCallback = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(1350328003, "NoModuleManifestUsingDefaults"), "No module manifest at '{manifestPath}'. Using defaults.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __NoModuleManifestUsingDefaultCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(498258846, "NoModuleManifestUsingDefault"), "No module manifest at '{manifestPath}' using default", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Analyzing '{path}' for modules")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AnalyzingPathForModules(this ILogger<ModuleFinder> logger, string path)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__AnalyzingPathForModulesCallback(logger, path, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "Not analyzing directories in modules due to VirtualPathProvider being null")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void NotAnalyzingDirectoriesNullProvider(this ILogger<ModuleFinder> logger)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__NotAnalyzingDirectoriesNullProviderCallback(logger, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Ignoring '{folderName}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void IgnoringProtectedFolder(this ILogger<ModuleFinder> logger, string folderName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__IgnoringProtectedFolderCallback(logger, folderName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Looking for module in '{modulePath}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LookingForModuleInPath(this ILogger<ModuleFinder> logger, string modulePath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__LookingForModuleInPathCallback(logger, modulePath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "Not analyzing '{modulePath}' in modules due to VirtualPathProvider being null")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void NotAnalyzingModulePathNullProvider(this ILogger<ModuleFinder> logger, string modulePath)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__NotAnalyzingModulePathNullProviderCallback(logger, modulePath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "The modules finder couldn't find a directory at '{modulePath}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ModuleDirectoryNotFound(this ILogger<ModuleFinder> logger, string modulePath)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__ModuleDirectoryNotFoundCallback(logger, modulePath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Found module: {module}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FoundModule(this ILogger<ModuleFinder> logger, object module)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__FoundModuleCallback(logger, module, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Checking assembly '{assemblyName}' configured in web.config for module in directory '{moduleDir}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void CheckingAssemblyConfiguredInWebConfig(this ILogger<ModuleFinder> logger, string assemblyName, string moduleDir)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__CheckingAssemblyConfiguredInWebConfigCallback(logger, assemblyName, moduleDir, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Checking assembly '{assemblyName}' configured in manifest from module in directory '{moduleDir}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void CheckingAssemblyConfiguredInManifest(this ILogger<ModuleFinder> logger, string assemblyName, string moduleDir)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__CheckingAssemblyConfiguredInManifestCallback(logger, assemblyName, moduleDir, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Looking for assemblies in module directory {moduleDir}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LookingForAssembliesInModuleDirectory(this ILogger<ModuleFinder> logger, string moduleDir)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__LookingForAssembliesInModuleDirectoryCallback(logger, moduleDir, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Loading modules assembly {assemblyName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LoadingModulesAssembly(this ILogger<ModuleFinder> logger, string assemblyName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__LoadingModulesAssemblyCallback(logger, assemblyName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Try to load assembly by name '{assemblyName}'.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void TryToLoadAssemblyByName(this ILogger<ModuleFinder> logger, string assemblyName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__TryToLoadAssemblyByNameCallback(logger, assemblyName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Reading assembly from binaries directory: {dllPath}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ReadingAssemblyFromBinariesDirectory(this ILogger<ModuleFinder> logger, string dllPath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__ReadingAssemblyFromBinariesDirectoryCallback(logger, dllPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Loading '{bytesCount}' bytes of assembly from library '{assemblyName}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LoadingBytesOfAssembly(this ILogger<ModuleFinder> logger, int bytesCount, string assemblyName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__LoadingBytesOfAssemblyCallback(logger, bytesCount, assemblyName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Assembly {assemblyFullName} is already registered for module, skipping it.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AssemblyAlreadyRegistered(this ILogger<ModuleFinder> logger, string assemblyFullName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__AssemblyAlreadyRegisteredCallback(logger, assemblyFullName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Registering assembly {assemblyFullName} for module.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void RegisteringAssembly(this ILogger<ModuleFinder> logger, string assemblyFullName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__RegisteringAssemblyCallback(logger, assemblyFullName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Unable to load module assembly by name.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void UnableToLoadModuleAssemblyByName(this ILogger<ModuleFinder> logger, Exception exception)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__UnableToLoadModuleAssemblyByNameCallback(logger, exception);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Reading manifest from: {manifestPath}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ReadingManifest(this ILogger<ModuleFinder> logger, string manifestPath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__ReadingManifestCallback(logger, manifestPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "No module manifest at '{manifestPath}'. Using defaults.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void NoModuleManifestUsingDefaults(this ILogger<ModuleFinder> logger, string manifestPath)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__NoModuleManifestUsingDefaultsCallback(logger, manifestPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "No module manifest at '{manifestPath}' using default")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void NoModuleManifestUsingDefault(this ILogger<ModuleFinder> logger, string manifestPath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__NoModuleManifestUsingDefaultCallback(logger, manifestPath, null);
		}
	}
}

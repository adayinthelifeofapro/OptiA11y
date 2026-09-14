using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class BinariesFinderExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ModulesRootPathNotFoundCallback = LoggerMessage.Define<string>(LogLevel.Error, new EventId(1475065866, "ModulesRootPathNotFound"), "Modules root path {ModulesRootPath} not found", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ModuleFolderDoesNotExistCallback = LoggerMessage.Define<string>(LogLevel.Error, new EventId(1437641328, "ModuleFolderDoesNotExist"), "Module folder {ModuleFolder} does not exist.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __BinariesFolderDoesNotExistCallback = LoggerMessage.Define<string, string>(LogLevel.Information, new EventId(1989188479, "BinariesFolderDoesNotExist"), "Binaries folder {BinariesSubfolder} does not exist in module folder {ModuleFolder}.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Error, Message = "Modules root path {ModulesRootPath} not found")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ModulesRootPathNotFound(this ILogger<BinariesFinder> logger, string ModulesRootPath)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__ModulesRootPathNotFoundCallback(logger, ModulesRootPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "Module folder {ModuleFolder} does not exist.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ModuleFolderDoesNotExist(this ILogger<BinariesFinder> logger, string ModuleFolder)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__ModuleFolderDoesNotExistCallback(logger, ModuleFolder, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Binaries folder {BinariesSubfolder} does not exist in module folder {ModuleFolder}.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void BinariesFolderDoesNotExist(this ILogger<BinariesFinder> logger, string BinariesSubfolder, string ModuleFolder)
	{
		if (logger.IsEnabled(LogLevel.Information))
		{
			__BinariesFolderDoesNotExistCallback(logger, BinariesSubfolder, ModuleFolder, null);
		}
	}
}

using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class ModuleTableExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __InitializingCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(1864157236, "Initializing"), "Initializing", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __AddingModuleCallback = LoggerMessage.Define<string>(LogLevel.Information, new EventId(855928812, "AddingModule"), "Adding module {ModuleName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ReplacingExistingModuleCallback = LoggerMessage.Define<string>(LogLevel.Information, new EventId(114507791, "ReplacingExistingModule"), "Replacing existing module {ModuleName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Initializing")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void Initializing(this ILogger<ModuleTable> logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__InitializingCallback(logger, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Adding module {ModuleName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AddingModule(this ILogger<ModuleTable> logger, string moduleName)
	{
		if (logger.IsEnabled(LogLevel.Information))
		{
			__AddingModuleCallback(logger, moduleName, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Replacing existing module {ModuleName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ReplacingExistingModule(this ILogger<ModuleTable> logger, string moduleName)
	{
		if (logger.IsEnabled(LogLevel.Information))
		{
			__ReplacingExistingModuleCallback(logger, moduleName, null);
		}
	}
}

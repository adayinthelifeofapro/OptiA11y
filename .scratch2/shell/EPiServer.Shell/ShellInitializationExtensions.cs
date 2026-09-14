using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell;

internal static class ShellInitializationExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __InitializingShellModulesCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(1806941931, "InitializingShellModules"), "Initializing shell modules", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __ShellModuleInitializationCompleteCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(1159166166, "ShellModuleInitializationComplete"), "Shell module initialization complete", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __LogInitializationErrorCallback = LoggerMessage.Define(LogLevel.Error, new EventId(1859655009, "LogInitializationError"), "Error occurred while initializing shell modules", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Initializing shell modules")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void InitializingShellModules(this ILogger<ShellInitialization> logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__InitializingShellModulesCallback(logger, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Shell module initialization complete")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ShellModuleInitializationComplete(this ILogger<ShellInitialization> logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__ShellModuleInitializationCompleteCallback(logger, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "Error occurred while initializing shell modules")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LogInitializationError(this ILogger<ShellInitialization> logger, Exception exception)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__LogInitializationErrorCallback(logger, exception);
		}
	}
}

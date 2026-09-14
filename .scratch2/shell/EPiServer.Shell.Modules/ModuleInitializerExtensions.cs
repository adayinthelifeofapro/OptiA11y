using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class ModuleInitializerExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __AddingRouteCallback = LoggerMessage.Define<string, string>(LogLevel.Debug, new EventId(1739709431, "AddingRoute"), "Adding route '{url}' from module '{moduleName}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Adding route '{url}' from module '{moduleName}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AddingRoute(this ILogger<ModuleInitializer> logger, string url, string moduleName)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__AddingRouteCallback(logger, url, moduleName, null);
		}
	}
}

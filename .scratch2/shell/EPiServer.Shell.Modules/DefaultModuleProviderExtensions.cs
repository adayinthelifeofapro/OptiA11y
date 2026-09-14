using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class DefaultModuleProviderExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __FailedToGetModuleFromPathCallback = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(252276656, "FailedToGetModuleFromPath"), "Failed to get module from the path '{routeBasePath}', no default module will be available.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Warning, Message = "Failed to get module from the path '{routeBasePath}', no default module will be available.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FailedToGetModuleFromPath(this ILogger<DefaultModuleProvider> logger, string routeBasePath)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__FailedToGetModuleFromPathCallback(logger, routeBasePath, null);
		}
	}
}

using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class ConfigModuleProviderExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __AutoDiscoveringModulesCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(1207746976, "AutoDiscoveringModules"), "Auto discovering modules", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Auto discovering modules")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AutoDiscoveringModules(this ILogger<ConfigModuleProvider> logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__AutoDiscoveringModulesCallback(logger, null);
		}
	}
}

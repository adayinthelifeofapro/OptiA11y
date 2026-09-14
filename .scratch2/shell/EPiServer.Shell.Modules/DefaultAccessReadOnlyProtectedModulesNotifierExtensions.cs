using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class DefaultAccessReadOnlyProtectedModulesNotifierExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, object, Exception?> __AccessReadOnlyProtectedModuleHandledCallback = LoggerMessage.Define<object>(LogLevel.Debug, new EventId(352300903, "AccessReadOnlyProtectedModuleHandled"), "AccessReadOnlyProtectedModule has been handled by {EventHandlerMethod}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "AccessReadOnlyProtectedModule has been handled by {EventHandlerMethod}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void AccessReadOnlyProtectedModuleHandled(this ILogger<DefaultAccessReadOnlyProtectedModulesNotifier> logger, object EventHandlerMethod)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__AccessReadOnlyProtectedModuleHandledCallback(logger, EventHandlerMethod, null);
		}
	}
}

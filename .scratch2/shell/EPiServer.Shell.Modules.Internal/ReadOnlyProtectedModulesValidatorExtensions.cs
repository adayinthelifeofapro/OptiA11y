using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules.Internal;

internal static class ReadOnlyProtectedModulesValidatorExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ReadOnlyModeBlockingRequestCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1728708832, "ReadOnlyModeBlockingRequest"), "{Message}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "{Message}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ReadOnlyModeBlockingRequest(this ILogger<ReadOnlyProtectedModulesValidator> logger, string message)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__ReadOnlyModeBlockingRequestCallback(logger, message, null);
		}
	}
}

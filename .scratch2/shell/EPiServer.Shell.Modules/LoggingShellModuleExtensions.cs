using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class LoggingShellModuleExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __CouldNotFindFileForCultureCallback = LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(1623502001, "CouldNotFindFileForCulture"), "Could not find the file {fileUrl} for the current culture [{culture}]", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Warning, Message = "Could not find the file {fileUrl} for the current culture [{culture}]")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void CouldNotFindFileForCulture(this ILogger<ShellModule> logger, string fileUrl, string culture)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__CouldNotFindFileForCultureCallback(logger, fileUrl, culture, null);
		}
	}
}

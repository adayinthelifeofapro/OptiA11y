using System;
using System.CodeDom.Compiler;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell;

internal static class PathsExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __FailedToMakePathAbsoluteCallback = LoggerMessage.Define<string>(LogLevel.Error, new EventId(1937694043, "FailedToMakePathAbsolute"), "Failed to make the path: {Path} absolute.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Error, Message = "Failed to make the path: {Path} absolute.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FailedToMakePathAbsolute(this ILogger<ShellModule> logger, Exception exception, string path)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__FailedToMakePathAbsoluteCallback(logger, path, exception);
		}
	}
}

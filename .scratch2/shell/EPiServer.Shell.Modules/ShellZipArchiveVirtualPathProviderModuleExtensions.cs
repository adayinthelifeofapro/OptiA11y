using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class ShellZipArchiveVirtualPathProviderModuleExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __RegisteringZipArchiveVirtualPathProviderCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(923641224, "RegisteringZipArchiveVirtualPathProvider"), "Registering ZipArchiveVirtualPathProvider for module '{ArchiveKey}'", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Registering ZipArchiveVirtualPathProvider for module '{ArchiveKey}'")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void RegisteringZipArchiveVirtualPathProvider(this ILogger<ShellZipArchiveVirtualPathProviderModule> logger, string ArchiveKey)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__RegisteringZipArchiveVirtualPathProviderCallback(logger, ArchiveKey, null);
		}
	}
}

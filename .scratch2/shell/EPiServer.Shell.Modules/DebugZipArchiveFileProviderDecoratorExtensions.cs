using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal static class DebugZipArchiveFileProviderDecoratorExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __GetFileCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(671207991, "GetFile"), "GetFile {Subpath}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __FallingBackToUncompressedCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(139723740, "FallingBackToUncompressed"), "Falling back to uncompressed file {UncompressedPath}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __FallingBackToUnminifiedCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1866007038, "FallingBackToUnminified"), "Falling back to unminified file {UnminifiedPath}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "GetFile {Subpath}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void GetFile(this ILogger<DebugZipArchiveFileProviderDecorator> logger, string Subpath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__GetFileCallback(logger, Subpath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Falling back to uncompressed file {UncompressedPath}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FallingBackToUncompressed(this ILogger<DebugZipArchiveFileProviderDecorator> logger, string UncompressedPath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__FallingBackToUncompressedCallback(logger, UncompressedPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Falling back to unminified file {UnminifiedPath}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FallingBackToUnminified(this ILogger<DebugZipArchiveFileProviderDecorator> logger, string UnminifiedPath)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__FallingBackToUnminifiedCallback(logger, UnminifiedPath, null);
		}
	}
}

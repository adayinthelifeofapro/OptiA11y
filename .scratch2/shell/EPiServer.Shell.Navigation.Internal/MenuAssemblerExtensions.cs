using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Navigation.Internal;

internal static class MenuAssemblerExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __CouldNotFindParentPathCallback = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(670728191, "CouldNotFindParentPath"), "Could not find the parent path [{parentPath}]", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __FailedToAddMenuItemCallback = LoggerMessage.Define(LogLevel.Error, new EventId(1369141804, "FailedToAddMenuItem"), "Failed to add menu item", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Warning, Message = "Could not find the parent path [{parentPath}]")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void CouldNotFindParentPath(this ILogger<MenuAssembler> logger, string parentPath)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__CouldNotFindParentPathCallback(logger, parentPath, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "Failed to add menu item")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FailedToAddMenuItem(this ILogger<MenuAssembler> logger, Exception exception)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__FailedToAddMenuItemCallback(logger, exception);
		}
	}
}

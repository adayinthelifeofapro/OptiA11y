using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Navigation.Providers;

internal static class ReflectingMenuItemProviderExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, string, Exception?> __UsingMenuItemForActionCallback = LoggerMessage.Define<string, string, string>(LogLevel.Debug, new EventId(1414074560, "UsingMenuItemForAction"), "Using menu item for action '{ActionName}' on controller '{ControllerName}' with url: {Url}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __DidNotUseMenuItemForActionCallback = LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(904311727, "DidNotUseMenuItemForAction"), "Didn't use menu item for action '{ActionName}' on controller '{ControllerName}' due to route table not returning a virtual path for it", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Using menu item for action '{ActionName}' on controller '{ControllerName}' with url: {Url}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void UsingMenuItemForAction(this ILogger<ReflectingMenuItemProvider> logger, string actionName, string controllerName, string url)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__UsingMenuItemForActionCallback(logger, actionName, controllerName, url, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Warning, Message = "Didn't use menu item for action '{ActionName}' on controller '{ControllerName}' due to route table not returning a virtual path for it")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void DidNotUseMenuItemForAction(this ILogger<ReflectingMenuItemProvider> logger, string actionName, string controllerName)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__DidNotUseMenuItemForActionCallback(logger, actionName, controllerName, null);
		}
	}
}

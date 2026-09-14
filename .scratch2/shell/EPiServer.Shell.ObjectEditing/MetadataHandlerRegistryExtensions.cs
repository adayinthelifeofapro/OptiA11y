using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.ObjectEditing;

internal static class MetadataHandlerRegistryExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __OverrideDefaultBehaviorWarningCallback = LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(492896028, "OverrideDefaultBehaviorWarning"), "There are two editor descriptors marked as override default behavior: {SettingsType} and {FirstHandlerType}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Warning, Message = "There are two editor descriptors marked as override default behavior: {SettingsType} and {FirstHandlerType}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void OverrideDefaultBehaviorWarning(this ILogger<MetadataHandlerRegistry> logger, string settingsType, string firstHandlerType)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__OverrideDefaultBehaviorWarningCallback(logger, settingsType, firstHandlerType, null);
		}
	}
}

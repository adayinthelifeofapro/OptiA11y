using System;
using System.CodeDom.Compiler;
using EPiServer.Shell.ViewComposition;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Storage;

internal static class PersonalizedViewSettingsStorageExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __CouldNotLoadPersonalizedComponentCallback = LoggerMessage.Define<string, string>(LogLevel.Warning, new EventId(885992368, "CouldNotLoadPersonalizedComponent"), "Could not load personalized component with definition '{DefinitionName}' and view '{ViewName}'.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Warning, Message = "Could not load personalized component with definition '{DefinitionName}' and view '{ViewName}'.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void CouldNotLoadPersonalizedComponent(this ILogger<PersonalizedViewSettingsRepository> logger, string definitionName, string viewName)
	{
		if (logger.IsEnabled(LogLevel.Warning))
		{
			__CouldNotLoadPersonalizedComponentCallback(logger, definitionName, viewName, null);
		}
	}
}

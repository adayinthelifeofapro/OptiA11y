using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Search;

internal static class SearchProvidersManagerExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __ListingSearchProvidersCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(122008247, "ListingSearchProviders"), "Listing search providers within search area: {SearchArea}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __FailedToGetProviderCallback = LoggerMessage.Define<string>(LogLevel.Error, new EventId(412687854, "FailedToGetProvider"), "Failed to get provider with full name: {ProviderFullName}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, Exception?> __LoadingSearchSettingsCallback = LoggerMessage.Define(LogLevel.Debug, new EventId(430570710, "LoadingSearchSettings"), "Loading search settings", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "Listing search providers within search area: {SearchArea}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void ListingSearchProviders(this ILogger<SearchProviderManager> logger, string searchArea)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__ListingSearchProvidersCallback(logger, searchArea, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "Failed to get provider with full name: {ProviderFullName}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void FailedToGetProvider(this ILogger<SearchProviderManager> logger, Exception exception, string providerFullName)
	{
		if (logger.IsEnabled(LogLevel.Error))
		{
			__FailedToGetProviderCallback(logger, providerFullName, exception);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Loading search settings")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void LoadingSearchSettings(this ILogger<SearchProviderManager> logger)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__LoadingSearchSettingsCallback(logger, null);
		}
	}
}

using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell;

internal static class UIDescriptorRegistryExtensions
{
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, Exception?> __DuplicateUIDescriptorsDiscardedCallback = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1598096767, "DuplicateUIDescriptorsDiscarded"), "There are duplicated UIDescriptors defined. The following descriptors registrations will be discarded: {DiscardedDescriptors}", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	private static readonly Action<ILogger, string, string, Exception?> __UIDescriptorDiscardedCallback = LoggerMessage.Define<string, string>(LogLevel.Debug, new EventId(993922285, "UIDescriptorDiscarded"), "Discarding ui descriptor registration for type {TypeIdentifier} provided by {ProviderType} since a desciptor for the type is already registered.", new LogDefineOptions
	{
		SkipEnabledCheck = true
	});

	[LoggerMessage(Level = LogLevel.Debug, Message = "There are duplicated UIDescriptors defined. The following descriptors registrations will be discarded: {DiscardedDescriptors}")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void DuplicateUIDescriptorsDiscarded(this ILogger<UIDescriptorRegistry> logger, string discardedDescriptors)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__DuplicateUIDescriptorsDiscardedCallback(logger, discardedDescriptors, null);
		}
	}

	[LoggerMessage(Level = LogLevel.Debug, Message = "Discarding ui descriptor registration for type {TypeIdentifier} provided by {ProviderType} since a desciptor for the type is already registered.")]
	[GeneratedCode("Microsoft.Extensions.Logging.Generators", "10.0.14.42308")]
	public static void UIDescriptorDiscarded(this ILogger<UIDescriptorRegistry> logger, string typeIdentifier, string providerType)
	{
		if (logger.IsEnabled(LogLevel.Debug))
		{
			__UIDescriptorDiscardedCallback(logger, typeIdentifier, providerType, null);
		}
	}
}

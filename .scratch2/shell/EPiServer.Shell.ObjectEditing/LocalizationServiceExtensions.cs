using System;
using EPiServer.Framework.Localization;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Localization service extension
///       </summary>
public static class LocalizationServiceExtensions
{
	/// <summary>
	///       Gets the translation if needed.
	///       </summary>
	/// <param name="localizationService">The localization service.</param>
	/// <param name="resourceKey">The resource key.</param>
	/// <returns>
	/// </returns>
	public static string GetTranslationIfNeeded(this LocalizationService localizationService, string resourceKey)
	{
		if (resourceKey == null || !resourceKey.StartsWith("/", StringComparison.OrdinalIgnoreCase))
		{
			return resourceKey;
		}
		return localizationService.GetString(resourceKey);
	}
}

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.Localization;

namespace EPiServer.Shell.Resources;

/// <summary>
///       Provides methods for simplified resource handling.
///       </summary>
internal static class Resource
{
	/// <summary>
	///       Gets the resource value with fallback.
	///       </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="resourceType">Type of the resource.</param>
	/// <param name="fallbacks">Fallback values to use no resource key is set.</param>
	/// <returns>
	/// </returns>
	/// <remarks>If <paramref name="resourceType" /> is <c>null</c>, <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" />
	///       will be used to try to find a value for <paramref name="resourceKey" />.</remarks>
	public static string GetValueWithFallback(string resourceKey, Type resourceType, params string[] fallbacks)
	{
		return GetValueWithFallback(resourceKey, resourceType, LocalizationService.Current, fallbacks);
	}

	/// <summary>
	///       Gets the resource value with fallback.
	///       </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="resourceType">Type of the resource.</param>
	/// <param name="localizationService">The service used for localization.</param>
	/// <param name="fallbacks">Fallback values to use no resource key is set.</param>
	/// <returns>
	/// </returns>
	/// <remarks>If <paramref name="resourceType" /> is <c>null</c>, <paramref name="localizationService" />
	///       will be used to try to find a value for <paramref name="resourceKey" />.</remarks>
	/// <exception cref="T:System.ArgumentNullException">If <paramref name="localizationService" /> is <c>null</c>.</exception>
	public static string GetValueWithFallback(string resourceKey, Type resourceType, LocalizationService localizationService, params string[] fallbacks)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(localizationService, "localizationService");
		string text = null;
		if (resourceType != null && !string.IsNullOrEmpty(resourceKey))
		{
			text = (resourceType.GetProperty(resourceKey, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The ResourceType '{0}' does not define a static property '{1}'", resourceType, resourceKey))).GetValue(null, null) as string;
		}
		else if (resourceType == null && !string.IsNullOrEmpty(resourceKey))
		{
			text = localizationService.GetString(resourceKey, (FallbackBehaviors)8);
			if (text == null && ((Enum)localizationService.FallbackBehavior).HasFlag((Enum)(object)(FallbackBehaviors)4))
			{
				localizationService.TryGetStringByCulture(resourceKey, localizationService.FallbackCulture, ref text);
			}
		}
		return text ?? fallbacks.FirstOrDefault((string f) => !string.IsNullOrEmpty(f));
	}
}

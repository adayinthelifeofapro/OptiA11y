using System;
using EPiServer.Framework.Localization;

namespace EPiServer.Shell.Web;

/// <summary>
///       Extension methods for the DateTime class.
///       </summary>
public static class DateTimeExtensions
{
	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <returns>A text representation of the date.</returns>
	public static string ToFriendlyDateString(this DateTime? date)
	{
		if (!date.HasValue)
		{
			return "";
		}
		return date.Value.ToFriendlyDateString();
	}

	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <returns>A text representation of the date.</returns>
	public static string ToFriendlyDateString(this DateTime date)
	{
		return date.ToFriendlyDateString(LocalizationService.Current);
	}

	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <param name="localizationService">The service used for localization.</param>
	/// <returns>A text representation of the date.</returns>
	/// <remarks>If <paramref name="localizationService" /> is <c>null</c>, 
	///       the static instance <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" /> will be used.</remarks>
	public static string ToFriendlyDateString(this DateTime date, LocalizationService localizationService)
	{
		if (localizationService == null)
		{
			localizationService = LocalizationService.Current;
		}
		if (date.Date == DateTime.Today)
		{
			return localizationService.GetString("/EPiServer/Shell/Resources/Texts/Today");
		}
		if (date.Date == DateTime.Today.AddDays(-1.0))
		{
			return localizationService.GetString("/EPiServer/Shell/Resources/Texts/Yesterday");
		}
		return date.ToShortDateString();
	}

	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <returns>A text representation of the date.</returns>
	public static string ToFriendlyDateTimeString(this DateTime? date)
	{
		if (!date.HasValue)
		{
			return "";
		}
		return date.Value.ToFriendlyDateTimeString();
	}

	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <returns>A text representation of the date.</returns>
	public static string ToFriendlyDateTimeString(this DateTime date)
	{
		return date.ToFriendlyDateTimeString(LocalizationService.Current);
	}

	/// <summary>
	///       Returns Today, Yesterday or <see cref="M:System.DateTime.ToShortDateString" />.
	///       </summary>
	/// <param name="date">The date to transform.</param>
	/// <param name="localizationService">The service used for localization.</param>
	/// <returns>A text representation of the date.</returns>
	/// <remarks>If <paramref name="localizationService" /> is <c>null</c>, 
	///       the static instance <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" /> will be used.</remarks>
	public static string ToFriendlyDateTimeString(this DateTime date, LocalizationService localizationService)
	{
		if (localizationService == null)
		{
			localizationService = LocalizationService.Current;
		}
		if (date.Date == DateTime.Today)
		{
			return localizationService.GetString("/EPiServer/Shell/Resources/Texts/Today") + " " + date.ToShortTimeString();
		}
		if (date.Date == DateTime.Today.AddDays(-1.0))
		{
			return localizationService.GetString("/EPiServer/Shell/Resources/Texts/Yesterday") + " " + date.ToShortTimeString();
		}
		return date.ToString();
	}
}

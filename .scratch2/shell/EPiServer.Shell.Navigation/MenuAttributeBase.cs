using System;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Resources;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Base class for attributes that builds up the navigation.
///       </summary>
public abstract class MenuAttributeBase : Attribute
{
	/// <summary>
	///       Text to display in the menu
	///       </summary>
	public string Text { get; set; }

	/// <summary>
	///       Unique path for the menu item
	///       </summary>
	/// <example>
	///       /top/forum/search
	///       </example>
	public string MenuPath { get; protected set; }

	/// <summary>
	///       Name of a resource key to use for localized menu text
	///       </summary>
	/// <remarks>This can either be a resource key to a resource provided by <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" />
	///       or the name of the static resource wrapper property if <see cref="P:EPiServer.Shell.Navigation.MenuAttributeBase.ResourceType" /> is set.</remarks>
	public string TextResourceKey { get; set; }

	/// <summary>
	///       The strongly typed resource wrapper to use for resource string lookups
	///       </summary>
	/// <remarks>
	///   <para>If not set the <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" /> 
	///       will be used to find a match for <see cref="P:EPiServer.Shell.Navigation.MenuAttributeBase.TextResourceKey" />.</para>
	/// </remarks>
	public Type ResourceType { get; set; }

	/// <summary>
	///       An index for ordering menu items. See <see cref="P:EPiServer.Shell.Navigation.MenuAttributeBase.SortIndex" /> values for predefined values.
	///       </summary>
	public int SortIndex { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuAttributeBase" /> class.
	///       </summary>
	/// <param name="menuPath">The unique path of the menu item.</param>
	protected MenuAttributeBase(string menuPath)
	{
		MenuPath = menuPath;
	}

	/// <summary>
	///       Gets the localized text or the fallback text if no localized text is found.
	///       </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="fallbackText">The fallback text.</param>
	/// <remarks>This uses the static instance <see cref="P:EPiServer.Framework.Localization.LocalizationService.Current" /> 
	///       if <see cref="P:EPiServer.Shell.Navigation.MenuAttributeBase.ResourceType" /> is <c>null</c>.</remarks>
	public virtual string GetLocalizedText(string resourceKey, string fallbackText)
	{
		return GetLocalizedText(resourceKey, fallbackText, LocalizationService.Current);
	}

	/// <summary>
	///       Gets the localized text or the fallback text if no localized text is found.
	///       </summary>
	/// <param name="resourceKey">The resource key.</param>
	/// <param name="fallbackText">The fallback text.</param>
	/// <param name="localizationService">The service used for localization.</param>
	public virtual string GetLocalizedText(string resourceKey, string fallbackText, LocalizationService localizationService)
	{
		return Resource.GetValueWithFallback(resourceKey, ResourceType, localizationService, fallbackText);
	}
}

using System.Globalization;
using EPiServer.Framework.Localization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Extension methods for the <see cref="T:Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper" /> class that handles translation scenarios.
///       </summary>
public static class HtmlHelperTranslationExtensions
{
	private static LocalizationService LocalizationService(this IHtmlHelper html)
	{
		return html.ViewContext?.HttpContext?.RequestServices.GetService<LocalizationService>() ?? LocalizationService.Current;
	}

	/// <summary>
	///       Creates a HTML label using the supplied lang key
	///       </summary>
	/// <param name="html">The html helper to hook the method to</param>
	/// <param name="targetID">The label's for attribute value</param>
	/// <param name="translationKey">The lang file xpath from where to get label contents</param>
	/// <returns>A HTML label</returns>
	public static IHtmlContent TranslatedLabel(this IHtmlHelper html, string targetID, string translationKey)
	{
		TagBuilder tagBuilder = new TagBuilder("label");
		tagBuilder.Attributes["for"] = targetID;
		tagBuilder.InnerHtml.Append(html.LocalizationService().GetString(translationKey));
		return tagBuilder;
	}

	/// <summary>
	///       Creates a HTML input text with an associated translated label
	///       </summary>
	/// <param name="htmlHelper">The html helper to hook the method to</param>
	/// <param name="name">The name of the input text</param>
	/// <param name="translationKey">The lang file xpath from where to get label contents</param>
	/// <param name="value">The text box's value</param>
	/// <param name="htmlAttributes">Html attributes to decorate the input text</param>
	/// <returns>Two HTML elements, a label and an input text</returns>
	public static IHtmlContent TranslatedTextBox(this IHtmlHelper htmlHelper, string name, string translationKey, object value, object htmlAttributes)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		htmlContentBuilder.AppendHtml(htmlHelper.TranslatedLabel(name, translationKey)).AppendHtml(htmlHelper.TextBox(name, value, htmlAttributes));
		return htmlContentBuilder;
	}

	/// <summary>
	///       Creates an input with the appropriate styles and a translated button text
	///       </summary>
	public static IHtmlContent TranslatedButton(this IHtmlHelper html, string name, ShellInputType inputType, string translationKey, object htmlAttributes)
	{
		return html.ShellButton(inputType, name, html.LocalizationService().GetString(translationKey), null, htmlAttributes);
	}

	/// <summary>
	///       Translates using the current <see cref="M:EPiServer.Shell.Web.Mvc.Html.HtmlHelperTranslationExtensions.LocalizationService(Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper)" />.
	///       </summary>
	/// <param name="html">
	/// </param>
	/// <param name="translationKey">The xpath to the translated text</param>
	/// <returns>A translated string or a warning text if no translation was found</returns>
	public static IHtmlContent Translate(this IHtmlHelper html, string translationKey)
	{
		return new HtmlString(html.LocalizationService().GetString(translationKey));
	}

	/// <summary>
	///       Translates using the current <see cref="M:EPiServer.Shell.Web.Mvc.Html.HtmlHelperTranslationExtensions.LocalizationService(Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper)" />.
	///       </summary>
	/// <param name="html">
	/// </param>
	/// <param name="translationKey">The xpath to the translated text</param>
	/// <param name="values">An array of objects passed to the string.Format method</param>
	/// <returns>A translated string or a warning text if no translation was found</returns>
	public static IHtmlContent TranslateFormat(this IHtmlHelper html, string translationKey, params object[] values)
	{
		return new HtmlString(string.Format(CultureInfo.CurrentCulture, html.LocalizationService().GetString(translationKey), values));
	}

	/// <summary>
	///       Translates using the current <see cref="M:EPiServer.Shell.Web.Mvc.Html.HtmlHelperTranslationExtensions.LocalizationService(Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper)" />.
	///       </summary>
	/// <param name="html">
	/// </param>
	/// <param name="translationKey">The xpath to the translated text</param>
	/// <returns>A translated string or a warning text if no translation was found</returns>
	public static IHtmlContent TranslateForScript(this IHtmlHelper html, string translationKey)
	{
		return new HtmlString(ScriptResourceHelper.PrepareResourceForScript(html.LocalizationService().GetString(translationKey)));
	}

	/// <summary>
	///       Translate the given string to the current language. Will return supplied fallback string if no match is found.
	///       </summary>
	/// <param name="htmlHelper">The html helper to hook the method to</param>
	/// <param name="translationKey">A string to translate</param>
	/// <param name="fallback">The string to return if no match was found for key.</param>
	/// <returns>
	///       The translated string.
	///       </returns>
	public static IHtmlContent TranslateFallback(this IHtmlHelper htmlHelper, string translationKey, string fallback)
	{
		return new HtmlString(htmlHelper.LocalizationService().GetString(translationKey, fallback));
	}
}

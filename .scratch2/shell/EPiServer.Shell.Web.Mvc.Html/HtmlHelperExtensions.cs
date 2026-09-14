using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Uncategorized html helper extensions
///       </summary>
public static class HtmlHelperExtensions
{
	/// <summary>
	///       Adds autoupdate feature on the gadget.
	///       </summary>
	/// <param name="html">The HTML</param>
	/// <param name="delay">Delay in milliseconds</param>
	/// <param name="actionName">The name of the action to call on autoupdate</param>
	/// <returns>Span which saves value for autoupdate</returns>
	public static IHtmlContent AutoUpdate(this IHtmlHelper html, int delay, string actionName)
	{
		TagBuilder tagBuilder = new TagBuilder("span");
		tagBuilder.AddCssClass("autoupdate");
		tagBuilder.Attributes["title"] = delay.ToString(CultureInfo.InvariantCulture);
		if (!string.IsNullOrEmpty(actionName))
		{
			tagBuilder.Attributes["name"] = actionName;
		}
		return tagBuilder;
	}

	/// <summary>
	///       Adds autoupdate feature on the gadget (also is possible from the gadget js options)
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="delay">Delay in milliseconds</param>
	/// <returns>Span which saves value for autoupdate</returns>
	public static IHtmlContent AutoUpdate(this IHtmlHelper html, int delay)
	{
		return html.AutoUpdate(delay, null);
	}

	/// <summary>
	///       Adds autoupdate feature on the gadget (also is possible from the gadget js options)
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="delay">Time span for delay</param>
	/// <returns>Span which saves value for autoupdate</returns>
	public static IHtmlContent AutoUpdate(this IHtmlHelper html, TimeSpan delay)
	{
		return html.AutoUpdate((int)delay.TotalMilliseconds);
	}

	/// <summary>
	///       Adds autoupdate feature on the gadget (also is possible from the gadget js options)
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="delay">Time span for delay</param>
	/// <param name="actionName">The name of the action to call on autoupdate</param>
	/// <returns>Span which saves value for autoupdate</returns>
	public static IHtmlContent AutoUpdate(this IHtmlHelper html, TimeSpan delay, string actionName)
	{
		return html.AutoUpdate((int)delay.TotalMilliseconds, actionName);
	}

	/// <summary>
	///       Creates a anchor with surrounding spans to emulate the look of ShellButtons.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="href">The href.</param>
	/// <param name="target">The target.</param>
	/// <param name="title">The title.</param>
	/// <param name="text">The text.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <returns>An html string representing the anchor.</returns>
	public static IHtmlContent ShellLinkButton(this IHtmlHelper html, string href, string target, string title, string text, string cssClass)
	{
		return html.ShellLinkButton(href, target, title, text, cssClass, null);
	}

	/// <summary>
	///       Creates a anchor with surrounding spans to emulate the look of ShellButtons.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="href">The href.</param>
	/// <param name="target">The target.</param>
	/// <param name="title">The title.</param>
	/// <param name="text">The text.</param>
	/// <param name="cssClass">The CSS class.</param>
	/// <param name="htmlAttributes">Any extra html attribute that should be added to the tag.</param>
	/// <returns>An html string representing the anchor.</returns>
	public static IHtmlContent ShellLinkButton(this IHtmlHelper html, string href, string target, string title, string text, string cssClass, object htmlAttributes)
	{
		TagBuilder tagBuilder = html.Anchor(href, target, title, text);
		if (htmlAttributes != null)
		{
			if (htmlAttributes is IDictionary<string, object> values)
			{
				tagBuilder.MergeAttributes(new RouteValueDictionary(values), replaceExisting: true);
			}
			else
			{
				tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes), replaceExisting: true);
			}
		}
		if (href.StartsWith("#", StringComparison.OrdinalIgnoreCase) && !tagBuilder.Attributes.Keys.Contains<string>("onclick", StringComparer.OrdinalIgnoreCase))
		{
			tagBuilder.Attributes.Add("onclick", "return false;");
		}
		return html.AddShellButtonTags(tagBuilder, cssClass);
	}

	/// <summary>
	///       Renders the given data objects into a table row elements.
	///       </summary>
	/// <param name="html">The HTML.</param>
	/// <param name="tableRows">The table rows.</param>
	/// <param name="tableProperties">The table properties.</param>
	public static void RenderTableRows(this IHtmlHelper html, IEnumerable tableRows, params string[] tableProperties)
	{
		StreamWriter streamWriter = new StreamWriter(html.ViewContext.HttpContext.Response.Body);
		foreach (object tableRow in tableRows)
		{
			streamWriter.Write("<tr>");
			foreach (string name in tableProperties)
			{
				streamWriter.Write("<td>");
				streamWriter.Write(tableRow.GetType().GetProperty(name).GetValue(tableRow, null));
				streamWriter.Write("</td>");
			}
			streamWriter.Write("</tr>");
		}
		streamWriter.Flush();
	}

	/// <summary>
	///       Creates paging html elements.
	///       </summary>
	/// <param name="html">The object this method is attached to</param>
	/// <param name="status">The paging status.</param>
	/// <param name="action">The action that should be executed when a paging item is selected.</param>
	/// <returns>Html containing paging elements.</returns>
	public static IHtmlContent Paging(this IHtmlHelper html, Pager status, string action)
	{
		return html.Paging(status, action, new { });
	}

	/// <summary>
	///       Creates paging html elements.
	///       </summary>
	/// <param name="html">The object this method is attached to</param>
	/// <param name="status">The paging status.</param>
	/// <param name="action">The action that should be executed when a paging item is selected.</param>
	/// <param name="routeValues">The route values.</param>
	/// <returns>Html containing paging elements.</returns>
	public static IHtmlContent Paging(this IHtmlHelper html, Pager status, string action, object routeValues)
	{
		HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
		htmlContentBuilder.Append("<div class=\"epi-paging\">");
		foreach (PagerPosition visiblePage in status.VisiblePages)
		{
			if (visiblePage.IsCurrent)
			{
				TagBuilder tagBuilder = new TagBuilder("span");
				tagBuilder.InnerHtml.Append(visiblePage.Name);
				htmlContentBuilder.AppendHtml(tagBuilder);
			}
			else
			{
				RouteValueDictionary routeValueDictionary = new RouteValueDictionary(routeValues);
				routeValueDictionary.Add("currentPageIndex", visiblePage.Index);
				routeValueDictionary.Add("pageSize", status.PageSize);
				string unencoded = html.ActionLink(visiblePage.Name, action, routeValueDictionary).ToString();
				htmlContentBuilder.Append(unencoded);
			}
		}
		htmlContentBuilder.Append("</div>");
		return htmlContentBuilder;
	}

	/// <summary>
	///       Creates an anchor with the given attributes.
	///       </summary>
	/// <param name="html">The html helper to append the extension method to.</param>
	/// <param name="href">The url to link to.</param>
	/// <param name="target">The target frame.</param>
	/// <param name="title">The title/tool tip.</param>
	/// <param name="text">The link text.</param>
	/// <param name="maxTextLength">The maximum length of the link.</param>
	/// <returns>A loaded tag builder.</returns>
	public static TagBuilder Anchor(this IHtmlHelper html, string href, string target, string title, string text, int maxTextLength)
	{
		return html.Anchor(href, target, title, text.Ellipsis(maxTextLength));
	}

	/// <summary>
	///       Creates an anchor with the given attributes.
	///       </summary>
	/// <param name="html">The html helper to append the extension method to.</param>
	/// <param name="href">The url to link to.</param>
	/// <param name="target">The target frame.</param>
	/// <param name="title">The title/tool tip.</param>
	/// <param name="text">The link text.</param>
	/// <returns>A loaded tag builder.</returns>
	public static TagBuilder Anchor(this IHtmlHelper html, string href, string target, string title, string text)
	{
		TagBuilder tagBuilder = new TagBuilder("a");
		if (!string.IsNullOrEmpty(target))
		{
			tagBuilder.Attr("target", target);
		}
		tagBuilder.Attr("href", href);
		tagBuilder.Attr("title", title);
		tagBuilder.InnerHtml.Append(text);
		return tagBuilder;
	}

	/// <summary>
	///       Creates a tag builder with a css class.
	///       </summary>
	/// <param name="htmlHelper">
	/// </param>
	/// <param name="tagName">The html tag name</param>
	/// <param name="cssClass">The css class to assign the tag</param>
	/// <returns>A tag builder</returns>
	public static TagBuilder Tag(this IHtmlHelper htmlHelper, string tagName, string cssClass)
	{
		TagBuilder tagBuilder = new TagBuilder(tagName);
		tagBuilder.AddCssClass(cssClass);
		return tagBuilder;
	}

	/// <summary>
	///       Will wrap the standard asp.net validation summary with a div with the css class .epi-gadgetFeedback.
	///       </summary>
	/// <param name="htmlHelper">The HTML helper this method is attached to.</param>
	/// <returns>
	///       The standard asp.net validation summary surrounded by a div with the css class .epi-gadgetFeedback.
	///       </returns>
	public static string ShellValidationSummary(this IHtmlHelper htmlHelper)
	{
		if (htmlHelper.ViewData.ModelState.IsValid)
		{
			return string.Empty;
		}
		return string.Format(CultureInfo.InvariantCulture, "<div class=\"epi-feedbackContent-error epi-feedbackContent epi-gadgetError\">{0}</div>", htmlHelper.ValidationSummary());
	}
}

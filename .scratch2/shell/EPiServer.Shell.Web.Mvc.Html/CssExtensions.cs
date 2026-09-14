using System.IO;
using System.Text.Encodings.Web;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       CSS related html helper extensions
///       </summary>
public static class CssExtensions
{
	/// <summary>
	///       Renders a CSS link tag from a site center relative css file path.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="shellRelativePath">A path relative from the shell base ui path.</param>
	/// <returns>A link html tag.</returns>
	public static IHtmlContent ShellCssLink(this IHtmlHelper html, string shellRelativePath)
	{
		TagBuilder tagBuilder = new TagBuilder("link").Attr("rel", "stylesheet").Attr("href", Paths.ToShellClientResource(shellRelativePath));
		using StringWriter stringWriter = new StringWriter();
		tagBuilder.WriteTo(stringWriter, HtmlEncoder.Default);
		return new HtmlString(stringWriter.ToString());
	}

	/// <summary>
	///       Returns the the theme class supposed to be used in the documents body tag for based on the dojo theming model
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <returns>The configured theme class string.</returns>
	public static string ThemeClass(this IHtmlHelper html)
	{
		return ServiceProviderExtensions.GetInstance<UIOptions>(ServiceLocator.Current).ThemeClass;
	}
}

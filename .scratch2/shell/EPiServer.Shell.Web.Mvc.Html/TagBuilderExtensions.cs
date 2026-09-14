using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Shell.Web.Mvc.Html;

internal static class TagBuilderExtensions
{
	/// <summary>
	///       Adds an attribute to the tag builder.
	///       </summary>
	public static TagBuilder Attr(this TagBuilder builder, string attributeName, string attributeValue)
	{
		builder.Attributes[attributeName] = attributeValue;
		return builder;
	}
}

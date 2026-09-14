using System;
using System.Globalization;
using System.IO;
using System.Text;
using EPiServer.Framework.Serialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Client script related html helper extensions
///       </summary>
public static class ScriptExtensions
{
	/// <summary>
	///       Serializes the object using the registered serializer for the target content type.
	///       </summary>
	/// <param name="html">The html helper to hook the helper on.</param>
	/// <param name="value">The object that should be serialized.</param>
	/// <param name="contentType">The type of serializer to use.</param>
	/// <returns>A string containing the serialized objct</returns>
	/// <exception cref="T:System.ArgumentException">when no serializer is registered for <paramref name="contentType" /></exception>
	public static HtmlString SerializeObject(this IHtmlHelper html, object value, string contentType)
	{
		IObjectSerializer objectSerializer = ServiceProviderExtensions.GetInstance<IObjectSerializerFactory>(ServiceLocator.Current).GetSerializer(contentType) ?? throw new ArgumentException("No serializer registered for content type " + contentType, "contentType");
		StringBuilder stringBuilder = new StringBuilder();
		using (StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
		{
			objectSerializer.Serialize(textWriter, value);
		}
		return new HtmlString(stringBuilder.ToString().Replace("<", "&lt;").Replace(">", "&gt;"));
	}
}

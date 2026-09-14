using System.IO;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Formatters;
using EPiServer.Framework.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Reperesents Json data results wrapped in TextArea tag.
///       That wrapping is required if results are used in dojo.io.iframe http://dojotoolkit.org/reference-guide/dojo/io/iframe.html#additional-information
///       </summary>
public class WrappedJsonDataResults : JsonDataResult
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.WrappedJsonDataResults" /> class.
	///       </summary>
	public WrappedJsonDataResults(object data)
		: base(data)
	{
		base.ContentType = "text/html";
	}

	/// <inheritdoc />
	public override async Task ExecuteResultAsync(ActionContext context)
	{
		IBufferedResponseWriterFactory requiredService = context.HttpContext.RequestServices.GetRequiredService<IBufferedResponseWriterFactory>();
		IObjectSerializer requiredService2 = context.HttpContext.RequestServices.GetRequiredService<IObjectSerializer>();
		context.HttpContext.Response.ContentType = base.ContentType;
		using TextWriter streamWriter = requiredService.CreateWriter(context.HttpContext.Response.Body, Encoding.Default);
		streamWriter.Write("<html><body><textarea>");
		string value = requiredService2.Serialize(base.Value).Replace("&quot;", "\\\"").Replace("&", "&amp;");
		streamWriter.Write(value);
		streamWriter.Write("</textarea></body></html>");
		await streamWriter.FlushAsync();
	}
}

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Serves as a base class for encapsulating REST responses.
///       </summary>
public abstract class RestResultBase : ActionResult
{
	/// <summary>
	///       Optional parameter to define which <see cref="T:EPiServer.Formatters.IJsonOutputFormatter" /> to use. If not specified then it will be resolved
	///       from <see cref="T:EPiServer.Formatters.IJsonOutputFormatterRegistry" /></summary>
	public IJsonOutputFormatter ModuleOutputFormatter { get; set; }

	/// <summary>
	///       Gets or sets the content encoding.
	///       </summary>
	/// <value>The content encoding.</value>
	public Encoding ContentEncoding { get; set; }

	/// <summary>
	///       Gets or sets the data returned to the client as a result of the request.
	///       </summary>
	public object Data { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestResultBase" /> class.
	///       </summary>
	protected RestResultBase()
	{
	}

	/// <inheritdoc />
	public override async Task ExecuteResultAsync(ActionContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		HttpResponse response = context.HttpContext.Response;
		if (ContentEncoding != null)
		{
			response.Headers.Append(HeaderNames.ContentEncoding, ContentEncoding.EncodingName);
		}
		if (Data != null)
		{
			IJsonOutputFormatter val = ResolveFormatter(context);
			JsonOutputFormatterWriterContext val2 = new JsonOutputFormatterWriterContext(context.HttpContext, (Func<Stream, Encoding, TextWriter>)val.CreateWriter, Data.GetType(), Data);
			await val.WriteAsync((OutputFormatterWriteContext)(object)val2, (ResponseDecorator)null);
		}
	}

	private IJsonOutputFormatter ResolveFormatter(ActionContext context)
	{
		if (ModuleOutputFormatter != null)
		{
			return ModuleOutputFormatter;
		}
		return context.HttpContext.RequestServices.GetRequiredService<IJsonOutputFormatterRegistry>().Resolve((Type)((context as ControllerContext)?.ActionDescriptor.ControllerTypeInfo));
	}
}

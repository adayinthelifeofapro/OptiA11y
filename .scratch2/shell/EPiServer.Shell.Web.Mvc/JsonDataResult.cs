using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Formatters;
using EPiServer.Framework.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       A <see cref="T:Microsoft.AspNetCore.Mvc.JsonResult" /> that uses <see cref="T:EPiServer.Formatters.IJsonOutputFormatterRegistry" /> to resolve the formatter to use for serialization
///       </summary>
public class JsonDataResult : JsonResult
{
	/// <summary>
	///       Optional parameter to define which <see cref="T:EPiServer.Formatters.IJsonOutputFormatter" /> to use. If not specified then it will be resolved
	///       from <see cref="T:EPiServer.Formatters.IJsonOutputFormatterRegistry" /></summary>
	public IJsonOutputFormatter ModuleOutputFormatter { get; set; }

	/// <inheritdoc />
	public JsonDataResult(object value)
		: base(value)
	{
	}

	/// <inheritdoc />
	public override async Task ExecuteResultAsync(ActionContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		context.HttpContext.Response.ContentType = base.ContentType ?? "application/json";
		if (base.Value != null)
		{
			IJsonOutputFormatter val = ResolveFormatter(context);
			context.HttpContext.RequestServices.GetRequiredService<ClientResourceOptions>();
			JsonOutputFormatterWriterContext val2 = new JsonOutputFormatterWriterContext(context.HttpContext, (Func<Stream, Encoding, TextWriter>)val.CreateWriter, base.Value.GetType(), base.Value);
			ResponseDecorator val3 = new ResponseDecorator();
			SetupDecorator(val3);
			await val.WriteAsync((OutputFormatterWriteContext)(object)val2, val3);
		}
	}

	/// <summary>
	///       Override to write something before data and/or to response
	///       </summary>
	/// <param name="responseDecorator">
	/// </param>
	protected virtual void SetupDecorator(ResponseDecorator responseDecorator)
	{
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

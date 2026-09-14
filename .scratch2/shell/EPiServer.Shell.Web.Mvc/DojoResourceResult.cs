using System.IO;
using EPiServer.Formatters;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Mvc <see cref="T:Microsoft.AspNetCore.Mvc.ActionResult" /> outputting JavaScript formatted Dojo resource files using a json serializer.
///       </summary>
public class DojoResourceResult : JsonDataResult
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.DojoResourceResult" /> class.
	///       </summary>
	public DojoResourceResult(object value)
		: base(value)
	{
		base.ContentType = "text/javascript";
	}

	/// <inheritdoc />
	protected override void SetupDecorator(ResponseDecorator responseDecorator)
	{
		responseDecorator.ContentType = base.ContentType;
		responseDecorator.PrefixWriter = async delegate(TextWriter writer)
		{
			await writer.WriteAsync("define(");
		};
		responseDecorator.SuffixWriter = async delegate(TextWriter writer)
		{
			await writer.WriteAsync(");");
		};
	}
}

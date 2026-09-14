using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Encapsulates the result of a successful REST request
///       </summary>
public class RestResult : RestResultBase
{
	/// <summary>
	///       Gets or sets information about the data range returned for the request.
	///       </summary>
	public ItemRange Range { get; set; }

	/// <summary>
	///       Processes the result and writes the resulting data to the http output stream.
	///       </summary>
	/// <param name="context">The context in which the result is executed.</param>
	public override async Task ExecuteResultAsync(ActionContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		if (!context.HttpContext.Response.HasStarted)
		{
			Range?.AddHeaderTo(context.HttpContext.Response);
		}
		await base.ExecuteResultAsync(context);
	}
}

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Provides a way to return a status code and description along with custom pay-load from a REST store
///       </summary>
public class RestStatusCodeResult : RestResultBase
{
	/// <summary>
	///       The HTTP status code returned in the response.
	///       </summary>
	public HttpStatusCode HttpStatusCode => (HttpStatusCode)StatusCode;

	/// <summary>
	///       The HTTP status code returned in the response.
	///       </summary>
	public int StatusCode { get; set; }

	/// <summary>
	///       A description of the HTTP status returned in the response.
	///       </summary>
	public string StatusDescription { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestStatusCodeResult" /> class.
	///       </summary>
	/// <param name="statusCode">The status code.</param>
	public RestStatusCodeResult(int statusCode)
		: this(statusCode, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestStatusCodeResult" /> class.
	///       </summary>
	/// <param name="statusCode">The status code.</param>
	public RestStatusCodeResult(HttpStatusCode statusCode)
		: this(statusCode, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestStatusCodeResult" /> class.
	///       </summary>
	/// <param name="statusCode">The status code.</param>
	/// <param name="statusDescription">The status description.</param>
	public RestStatusCodeResult(HttpStatusCode statusCode, string statusDescription)
		: this((int)statusCode, statusDescription)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestStatusCodeResult" /> class.
	///       </summary>
	/// <param name="statusCode">The status code.</param>
	/// <param name="data">Data returned to the client as a result.</param>
	public RestStatusCodeResult(HttpStatusCode statusCode, object data)
		: this((int)statusCode, string.Empty, data)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestStatusCodeResult" /> class.
	///       </summary>
	/// <param name="statusCode">The status code.</param>
	/// <param name="statusDescription">The status description.</param>
	/// <param name="data">Data returned to the client as a result.</param>
	public RestStatusCodeResult(int statusCode, string statusDescription, object data = null)
	{
		StatusCode = statusCode;
		StatusDescription = statusDescription;
		base.Data = data;
	}

	/// <inheritdoc />
	public override async Task ExecuteResultAsync(ActionContext context)
	{
		if (!context.HttpContext.Response.HasStarted)
		{
			context.HttpContext.Response.StatusCode = StatusCode;
		}
		await base.ExecuteResultAsync(context);
	}
}

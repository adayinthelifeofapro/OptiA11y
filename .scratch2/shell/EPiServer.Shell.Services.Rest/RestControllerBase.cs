using System.Collections.Generic;
using EPiServer.Shell.Web.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Serves as base class for controller implementations adhering to a REST pattern.
///       </summary>
[CompressFilter]
[ResponseCache(NoStore = true, Duration = -1, Location = ResponseCacheLocation.None)]
public abstract class RestControllerBase : Controller
{
	internal const string StoresUrlSegment = "stores";

	/// <summary>
	///       Helper method creating a <see cref="T:EPiServer.Shell.Services.Rest.RestResult" /> with the specified data.
	///       </summary>
	/// <param name="data">The data to return for the request.</param>
	/// <returns>A Rest Action result with the supplied data.</returns>
	protected RestResult Rest(object data)
	{
		return new RestResult
		{
			Data = data
		};
	}

	/// <summary>
	///       Helper method creating a <see cref="T:EPiServer.Shell.Services.Rest.RestResult" /> with the specified data filtered to a given range.
	///       </summary>
	/// <param name="data">The data to be filtered and then return in the response.</param>
	/// <param name="range">The item range to apply to the given data and apply to the response header.</param>
	/// <returns>A Rest Action result with the filtered data and Content-Range header.</returns>
	protected RestResult Rest(IEnumerable<object> data, ItemRange range)
	{
		RangedItems<object> rangedItems = data.ApplyRange(range);
		return new RestResult
		{
			Data = rangedItems.Items,
			Range = rangedItems.Range
		};
	}
}

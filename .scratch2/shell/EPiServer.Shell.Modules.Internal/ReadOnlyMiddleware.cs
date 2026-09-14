using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       Middleware called only when database is in readonly mode
///       </summary>
internal class ReadOnlyMiddleware
{
	private readonly RequestDelegate _next;

	private readonly ReadOnlyProtectedModulesValidator _readOnlyProtectedModulesValidator;

	public ReadOnlyMiddleware(RequestDelegate next, ReadOnlyProtectedModulesValidator readOnlyProtectedModulesValidator)
	{
		_next = next;
		_readOnlyProtectedModulesValidator = readOnlyProtectedModulesValidator;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		StringValues value;
		if (!string.Equals(context.Request.Method, HttpMethods.Get, StringComparison.InvariantCultureIgnoreCase))
		{
			await _next(context);
		}
		else if (context.Request.Headers.TryGetValue("Accept", out value) && !value.ToString().ToLowerInvariant().Contains("text/html"))
		{
			await _next(context);
		}
		else if (_readOnlyProtectedModulesValidator.Validate(context) != true)
		{
			await _next(context);
		}
	}
}

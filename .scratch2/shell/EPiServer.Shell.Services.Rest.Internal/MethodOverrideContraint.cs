using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EPiServer.Shell.Services.Rest.Internal;

internal class MethodOverrideContraint : IRouteConstraint, IParameterPolicy
{
	public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
	{
		return httpContext.Request.Headers.ContainsKey("X-Http-Method-Override");
	}
}

using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;

namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       Checks that the current request is a physical file
///       </summary>
public class StaticFileRouteContraint : IRouteConstraint, IParameterPolicy
{
	public const string ConstraintKey = "epi-staticfile";

	private readonly IFileProvider _fileProvider;

	public StaticFileRouteContraint(IFileProvider fileProvider)
	{
		_fileProvider = fileProvider;
	}

	public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
	{
		if (routeDirection == RouteDirection.UrlGeneration)
		{
			return false;
		}
		if (Path.HasExtension(httpContext.Request.Path))
		{
			return _fileProvider.GetFileInfo(httpContext.Request.Path).Exists;
		}
		return false;
	}
}

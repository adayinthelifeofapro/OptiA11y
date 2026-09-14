using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;

namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       Checks a route value representing a dojo localization file path against a pattern
///       and checks that the request doesn't map to an existing file on disk.
///       </summary>
public class DojoLocalizationFileRouteContraint : IRouteConstraint, IParameterPolicy
{
	public const string ConstraintKey = "epi-dojo-nls";

	private readonly Regex _nlsPathExpression = new Regex(".*nls/([a-z]{2}(-[a-z]{2,4})?/)?[^/]+\\.js$");

	private readonly IFileProvider _fileProvider;

	public DojoLocalizationFileRouteContraint(IFileProvider fileProvider)
	{
		_fileProvider = fileProvider;
	}

	public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
	{
		if (routeDirection == RouteDirection.UrlGeneration)
		{
			return false;
		}
		if (!values.TryGetValue(routeKey, out object value))
		{
			return false;
		}
		string text = value as string;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (!_nlsPathExpression.IsMatch(text))
		{
			return false;
		}
		return !_fileProvider.GetFileInfo(httpContext.Request.Path).Exists;
	}
}

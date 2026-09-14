using System.Globalization;
using System.Threading;
using EPiServer.Shell.Profile.Internal;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Extension methods for the <see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> class.
///       </summary>
internal static class HttpContextExtensions
{
	/// <summary>
	///       Reads the Language property of the profile and sets current thread culture
	///       </summary>
	/// <param name="httpContext">The HTTP context.</param>
	/// <param name="currentUiCulture">the ui language service used to resolve the preferred culture</param>
	public static void SetCulture(this HttpContext httpContext, ICurrentUiCulture currentUiCulture)
	{
		if (httpContext.Items["EPiServer.Shell:CultureInitialized"] == null || !(bool)httpContext.Items["EPiServer.Shell:CultureInitialized"])
		{
			CultureInfo cultureInfo = currentUiCulture.Get(httpContext.User.Identity.Name);
			Thread.CurrentThread.CurrentUICulture = cultureInfo;
			if (cultureInfo.IsNeutralCulture)
			{
				cultureInfo = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
			}
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			httpContext.Items["EPiServer.Shell:CultureInitialized"] = true;
		}
	}
}

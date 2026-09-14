using System.IO;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Shell.Web;

/// <summary>
///       Extensions for validating log-on via ajax requests
///       </summary>
public static class LogOnExtensions
{
	/// <summary>
	///       Validates an a log-on submitted via ajax.
	///       </summary>
	/// <param name="httpContext">The http context.</param>
	/// <param name="validationMethod">The method that will validate the user.</param>
	/// <returns>True if the user was sucessfully validated.</returns>
	public static bool ValidateAjaxLogOn(this HttpContext httpContext, ValidateUserNamePassword validationMethod)
	{
		if (TryParseCredentials(httpContext, out var username, out var password) && validationMethod(username, password))
		{
			if (!bool.TryParse(httpContext.Request.Form["rememberme"], out var result))
			{
				result = false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	///       Checks whether the request contains log-on information.
	///       </summary>
	/// <param name="httpContext">The http context containing the request.</param>
	/// <returns>true if the request is a log on request.</returns>
	public static bool IsLogOnRequest(this HttpContext httpContext)
	{
		string username;
		string password;
		return TryParseCredentials(httpContext, out username, out password);
	}

	private static bool TryParseCredentials(HttpContext httpContext, out string username, out string password)
	{
		username = httpContext.Request.Form["username"];
		password = httpContext.Request.Form["password"];
		if (!string.IsNullOrEmpty(username))
		{
			return !string.IsNullOrEmpty(password);
		}
		return false;
	}

	/// <summary>
	///       Sends authentication status via json to the client.
	///       </summary>
	/// <param name="httpContext">The http context.</param>
	/// <param name="authenticationSuccessful">True if the authentication was successful and the client may continue it's business.</param>
	public static void SendAjaxLogOnStatus(this HttpContext httpContext, bool authenticationSuccessful)
	{
		new StreamWriter(httpContext.Response.Body).Write("{authenticationSuccessful:" + (authenticationSuccessful ? "true" : "false") + "}");
		httpContext.Response.ContentType = "application/json";
	}

	/// <summary>
	///       Adds validation headers to the response. This is used in 
	///       ajax scenarios to display a log-on screen without redirecting.
	///       </summary>
	/// <param name="httpContext">The http context.</param>
	public static void AddAjaxLogOnHeaders(this HttpContext httpContext)
	{
		httpContext.Response.Headers.Append("X-EPiLogOnScreen", "true");
		httpContext.Response.Headers.Append("X-EPiLogOnScreen-PostUrl", VirtualPathUtilityEx.ToAbsolute(httpContext.Request.Path.ToString()));
	}
}

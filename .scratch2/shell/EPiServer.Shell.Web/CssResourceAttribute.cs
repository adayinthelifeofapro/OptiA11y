using System;

namespace EPiServer.Shell.Web;

/// <summary>
///       Declares a required client css resource
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CssResourceAttribute : ClientResourceAttribute
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.CssResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL</param>
	public CssResourceAttribute(string resourceUrl)
		: base(resourceUrl)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.CssResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL.</param>
	/// <param name="moduleName">The module were the resource can be found.</param>
	public CssResourceAttribute(string resourceUrl, string moduleName)
		: base(resourceUrl, moduleName)
	{
	}
}

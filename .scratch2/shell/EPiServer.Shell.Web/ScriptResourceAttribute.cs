using System;

namespace EPiServer.Shell.Web;

/// <summary>
///       Declares a required client script resource
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class ScriptResourceAttribute : ClientResourceAttribute
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.ScriptResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL</param>
	public ScriptResourceAttribute(string resourceUrl)
		: base(resourceUrl)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.ScriptResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL.</param>
	/// <param name="moduleName">The module were the resource can be found.</param>
	public ScriptResourceAttribute(string resourceUrl, string moduleName)
		: base(resourceUrl, moduleName)
	{
	}
}

using System;
using System.Threading;

namespace EPiServer.Shell.Web;

/// <summary>
///       Declares a required client localization resources
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class LocalizationResourceAttribute : ClientResourceAttribute
{
	/// <summary>
	///       RouteValues for the localized resource
	///       </summary>
	public object RouteValues { get; private set; }

	/// <summary>
	///       Type used for created RouteValues localization
	///       </summary>
	public Type LocalizationType { get; private set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.LocalizationResourceAttribute" /> class.
	///       </summary>
	/// <param name="localizationType">Relative URL</param>
	public LocalizationResourceAttribute(Type localizationType)
		: base(string.Empty)
	{
		LocalizationType = localizationType;
		RouteValues = new
		{
			typeName = localizationType.FullName,
			culture = Thread.CurrentThread.CurrentUICulture.ToString()
		};
	}
}

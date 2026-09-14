using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Tries to create the <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> information parameter from the http request headers.
///       </summary>
public class RestHeaderValuesProvider : IRestControllerValueProvider
{
	/// <summary>
	///       Returns the value for the Range http header.
	///       </summary>
	/// <param name="controllerContext">The controller context.</param>
	/// <param name="parameterDescriptor">The parameter descriptor.</param>
	/// <returns>
	///       The parameter value if a match was made; otherwise false.
	///       </returns>
	public object GetParameterValue(ActionContext controllerContext, ParameterDescriptor parameterDescriptor)
	{
		if (typeof(ItemRange).IsAssignableFrom(parameterDescriptor.ParameterType))
		{
			return ItemRange.ReadHeaderFrom(controllerContext.HttpContext.Request);
		}
		return null;
	}
}

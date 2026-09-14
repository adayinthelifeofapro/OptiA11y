using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Interface for providing parameter values
///       </summary>
public interface IRestControllerValueProvider
{
	/// <summary>
	///       Returns the value for a specific parameter.
	///       </summary>
	/// <param name="controllerContext">The controller context.</param>
	/// <param name="parameterDescriptor">The parameter descriptor.</param>
	/// <returns>The parameter value if a match was made; otherwise false.</returns>
	object GetParameterValue(ActionContext controllerContext, ParameterDescriptor parameterDescriptor);
}

using System;

namespace EPiServer.Shell;

/// <summary>
///       Helper class for method parameter validation.
///       </summary>
[Obsolete("Replace Validate.RequiredParameter with System.ArgumentNullException.ThrowIfNull.")]
public static class Validate
{
	/// <summary>
	///       Helper method to validate parameters. Throws ArgumentNullException
	///       when the parameter is null.
	///       </summary>
	/// <param name="parameterName">The name of the parameter to check.</param>
	/// <param name="parameter">The parameter instance to check.</param>
	public static void RequiredParameter(string parameterName, object parameter)
	{
		if (parameter == null)
		{
			throw new ArgumentNullException(parameterName);
		}
	}
}

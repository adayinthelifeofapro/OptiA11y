using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Interface to support creating metadata model accessor from set of arguments
///       </summary>
public interface IModelAccessorCreator
{
	/// <summary>
	///       Creates the metadata model accessor.
	///       </summary>
	/// <param name="arguments">The arguments.</param>
	/// <returns>
	/// </returns>
	Func<object> Create(Dictionary<string, string> arguments);
}

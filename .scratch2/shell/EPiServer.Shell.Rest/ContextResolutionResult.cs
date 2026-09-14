using System.Collections.Generic;

namespace EPiServer.Shell.Rest;

/// <summary>
///       The result of a context query.
///       </summary>
public class ContextResolutionResult
{
	/// <summary>
	///       Gets or sets the resolved context.
	///       </summary>
	public ClientContextBase Context { get; set; }

	/// <summary>
	///       Gets or sets a collection of errors that occurred during context resolution.
	///       </summary>
	public IEnumerable<string> Errors { get; set; }

	/// <summary>
	///       Gets a value indicating whether the operation completed without errors.
	///       </summary>
	public bool Successful => Context != null;
}

using System.Collections.Generic;

namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes result of update user operation.
///       </summary>
public class UIUserActionResult
{
	/// <summary>
	///       The status of operation.
	///       </summary>
	public bool Status { get; set; }

	/// <summary>
	///       The errors of operation.
	///       </summary>
	public IEnumerable<string> Errors { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Security.UIUserActionResult" /> class.
	///       </summary>
	/// <param name="status">true if operation succeeded otherwise false.</param>
	/// <param name="errors">List of errors</param>
	public UIUserActionResult(bool status, IEnumerable<string> errors = null)
	{
		Status = status;
		Errors = errors;
	}
}

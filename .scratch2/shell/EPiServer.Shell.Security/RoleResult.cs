using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes result of create role operation.
///       </summary>
public class RoleResult
{
	/// <summary>
	///       Empty result
	///       </summary>
	public static readonly RoleResult Empty = new RoleResult(succeeded: true, Enumerable.Empty<string>());

	/// <summary>
	///       The status of role operation.
	///       </summary>
	public bool Succeeded { get; }

	/// <summary>
	///       The errors.
	///       </summary>
	public IEnumerable<string> Errors { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Security.CreateUserResult" /> class.
	///       </summary>
	/// <param name="succeeded">true if operation succeeded otherwise false.</param>
	/// <param name="errors">List of errors</param>
	public RoleResult(bool succeeded, IEnumerable<string> errors)
	{
		Succeeded = succeeded;
		Errors = errors;
	}
}

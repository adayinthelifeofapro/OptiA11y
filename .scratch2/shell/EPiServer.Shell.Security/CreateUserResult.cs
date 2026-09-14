using System.Collections.Generic;

namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes result of create user operation.
///       </summary>
public class CreateUserResult
{
	/// <summary>
	///       The created user.
	///       </summary>
	public IUIUser User { get; }

	/// <summary>
	///       The status of create user operation.
	///       </summary>
	public UIUserCreateStatus Status { get; }

	/// <summary>
	///       The errors.
	///       </summary>
	public IEnumerable<string> Errors { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Security.CreateUserResult" /> class.
	///       </summary>
	/// <param name="user">
	/// </param>
	/// <param name="status">
	/// </param>
	/// <param name="errors">
	/// </param>
	public CreateUserResult(IUIUser user, UIUserCreateStatus status, IEnumerable<string> errors)
	{
		User = user;
		Status = status;
		Errors = errors;
	}
}

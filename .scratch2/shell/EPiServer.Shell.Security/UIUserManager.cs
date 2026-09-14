using System;
using System.Threading.Tasks;

namespace EPiServer.Shell.Security;

/// <summary>
///        Manages users and its settings.
///       </summary>
public abstract class UIUserManager : IDisposable
{
	/// <summary>
	///        Resets a user's password to a new, automatically generated password.
	///       </summary>
	/// <returns>The Task that represents the asynchronous operation, containing the new password for the membership user.</returns>
	public virtual Task<string> ResetPasswordAsync(IUIUser user)
	{
		return Task.FromResult<string>(null);
	}

	/// <summary>
	///        Resets a user's password to a new password.
	///       </summary>
	/// <returns>The Task that represents the asynchronous operation, containing true if the update was successful; otherwise false.</returns>
	public virtual Task<bool> ResetPasswordAsync(IUIUser user, string newPassword)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Updates the password for the  user in the user provider data store.
	///       </summary>
	/// <param name="user">The User</param>
	/// <param name="oldPassword">The old password</param>
	/// <param name="newPassword">The new password</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the update was successful; otherwise false.</returns>
	public virtual Task<bool> ChangePasswordAsync(IUIUser user, string oldPassword, string newPassword)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Updates the password question and answer for the user in the user provider data store.
	///       </summary>
	/// <param name="user">The User</param>
	/// <param name="password">The current password for the membership user.</param>
	/// <param name="newPasswordQuestion"> The new password question value for the membership user.</param>
	/// <param name="newPasswordAnswer">The new password answer value for the membership user.</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the update was successful; otherwise false.</returns>
	public virtual Task<bool> ChangePasswordQuestionAndAnswerAsync(IUIUser user, string password, string newPasswordQuestion, string newPasswordAnswer)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Clears the locked-out state of the user so that the user provider can be validated.        
	///       </summary>
	/// <param name="user">The User</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the update was successful; otherwise false.</returns>
	public virtual Task<bool> UnlockUserAsync(IUIUser user)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Validates the password for the user.
	///       </summary>
	/// <param name="user">The User</param>
	/// <param name="password">The password to validate</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the password is valid; otherwise false.</returns>
	public virtual Task<bool> CheckPasswordAsync(IUIUser user, string password)
	{
		return Task.FromResult(result: false);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///       Disposes the manager
	///       </summary>
	/// <param name="disposing">true if dispaosing</param>
	protected virtual void Dispose(bool disposing)
	{
	}
}

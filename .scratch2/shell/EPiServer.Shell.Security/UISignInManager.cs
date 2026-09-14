using System;
using System.Threading.Tasks;

namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes and updates user information.
///       </summary>
public abstract class UISignInManager : IDisposable
{
	/// <summary>
	///        Verify the user
	///       </summary>
	/// <param name="username">The user name</param>
	/// <param name="password">The password</param>
	/// <returns>The Task that represents the asynchronous operation. true if succeeded otherwise false.</returns>
	public virtual Task<bool> SignInAsync(string username, string password)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Signs out the current user.
	///       </summary>
	/// <returns>The Task that represents the asynchronous operation. true if succeeded otherwise false.</returns>
	public virtual Task SignOutAsync()
	{
		return Task.CompletedTask;
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
	/// <param name="disposing">true if disposing</param>
	protected virtual void Dispose(bool disposing)
	{
	}
}

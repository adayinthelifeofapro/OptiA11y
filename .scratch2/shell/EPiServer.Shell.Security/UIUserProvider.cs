using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Security;

namespace EPiServer.Shell.Security;

/// <summary>
///        Manages users and its settings.
///       </summary>
public abstract class UIUserProvider : IDisposable
{
	private static readonly UIUserComparer _userComparer = new UIUserComparer();

	/// <summary>
	///       Indicating whether User management is enabled.
	///       </summary>
	public abstract bool Enabled { get; }

	/// <summary>
	///       The name of provider
	///       </summary>
	public abstract string Name { get; }

	/// <summary>
	///        Gets the minimum length required for a password.
	///       </summary>
	public virtual int MinRequiredPasswordLength => 6;

	/// <summary>
	///       Gets a value indicating whether the user provider is configured to require a unique e-mail address for each user name.
	///       </summary>
	public virtual bool RequiresUniqueEmail => true;

	/// <summary>
	///       Gets a value indicating whether the current user provider is configured to allow users to reset their passwords.
	///       </summary>
	public virtual bool EnablePasswordReset => false;

	/// <summary>
	///        Gets the minimum number of special characters that must be present in a valid password.
	///       </summary>
	public virtual int MinRequiredNonAlphanumericCharacters => 0;

	/// <summary>
	///        Gets a value indicating whether the user provider requires the user to answer a password question for password reset and retrieval.
	///       </summary>
	public virtual bool RequiresQuestionAndAnswer => false;

	/// <summary>
	///        Gets the wildcard symbol from the user provider.
	///       </summary>
	/// <returns>
	/// </returns>
	public virtual string GetWildcardSymbolFromDefaultProvider()
	{
		return "";
	}

	/// <summary>
	///       Determines whether a provider supports a specific EPiServer.Security.ProviderActions.
	///       </summary>
	/// <param name="providerName">The provider name</param>
	/// <param name="action">Action on provider EPiServer.Security.ProviderActions</param>
	/// <returns> Returns true if action on provider is supported otherwise false.</returns>
	public virtual bool IsSupported(string providerName, ProviderActions action)
	{
		return IsSupported(action);
	}

	/// <summary>
	///         Determines whether the specified provider has support for the property is on the User model.
	///        </summary>
	/// <param name="providerName">The provider name</param>
	/// <param name="propertyName">The property name</param>
	/// <returns> Returns true if the provider supports  otherwise false.</returns>
	/// <remarks>Default we have 2 properties defined email and comment.</remarks>
	public virtual bool IsSupported(string providerName, string propertyName)
	{
		return IsSupported(propertyName);
	}

	/// <summary>
	///       Determines whether a provider supports a specific EPiServer.Security.ProviderActions.
	///       </summary>
	/// <param name="action">Action on provider EPiServer.Security.ProviderActions</param>
	/// <returns> Returns true if action on provider is supported otherwise false.</returns>
	public virtual bool IsSupported(ProviderActions action)
	{
		return false;
	}

	/// <summary>
	///         Determines whether the provider has support for the property is on the User model.
	///        </summary>
	/// <param name="propertyName">The property name</param>
	/// <returns> Returns true if the provider supports  otherwise false.</returns>
	/// <remarks>Default we have 2 properties defined email and comment.</remarks>
	public virtual bool IsSupported(string propertyName)
	{
		return false;
	}

	/// <summary>
	///        Adds a new user with specified property values and returns a status parameter indicating that the user was successfully created or the reason the user creation failed.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <param name="password">The password</param>
	/// <param name="email">The email</param>
	/// <param name="passwordQuestion">the password question</param>
	/// <param name="passwordAnswer">The password-answer</param>
	/// <param name="isApproved"> A Boolean that indicates whether the new user is approved to log on.</param>
	/// <returns>The Task that represents the asynchronous operation, containing the IUIUser object and result operation for the newly created user.</returns>
	public virtual Task<CreateUserResult> CreateUserAsync(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved)
	{
		return Task.FromResult(new CreateUserResult(null, UIUserCreateStatus.ProviderError, Array.Empty<string>()));
	}

	/// <summary>
	///        Gets a collection of IUserBase users, in a page of data, where the user name contains the specified user name to match.
	///       </summary>
	/// <param name="usernameToMatch">The user name to search for.</param>
	/// <param name="pageIndex"> The index of the page of results to return. pageIndex is zero-based.</param>
	/// <param name="pageSize">The size of the page of results to return.</param>
	/// <returns>The Task that represents the asynchronous operation, containing a list of IUserBase that contains a page of pageSize beginning at the page specified by pageIndex.</returns>
	public virtual IAsyncEnumerable<IUIUser> FindUsersByNameAsync(string usernameToMatch, int pageIndex, int pageSize)
	{
		return AsyncEnumerable.Empty<IUIUser>();
	}

	/// <summary>
	///        Gets a collection of IUserBase , in a page of data, where the e-mail address contains the specified e-mail address to match.
	///       </summary>
	/// <param name="emailToMatch"> The e-mail address to search for</param>
	/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
	/// <param name="pageSize"> The size of the page of results to return.</param>
	/// <returns>The Task that represents the asynchronous operation, containing a list of IUserBase that contains a page of pageSize beginning at the page specified by pageIndex.</returns>
	public virtual IAsyncEnumerable<IUIUser> FindUsersByEmailAsync(string emailToMatch, int pageIndex, int pageSize)
	{
		return AsyncEnumerable.Empty<IUIUser>();
	}

	/// <summary>
	///         Gets a collection of all the users.
	///       </summary>
	/// <param name="pageIndex"> The index of the page of results to return. Use 0 to indicate the first page.</param>
	/// <param name="pageSize">The size of the page of results to return. pageIndex is zero-based.</param>
	/// <returns>The Task that represents the asynchronous operation, containing a list of IUserBase objects representing all the users.</returns>
	public virtual IAsyncEnumerable<IUIUser> GetAllUsersAsync(int pageIndex, int pageSize)
	{
		return AsyncEnumerable.Empty<IUIUser>();
	}

	/// <summary>
	///         Gets specified IUserBase.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <returns>The Task that represents the asynchronous operation, containing an IUserBase object representing the specified user. If the username parameter does not correspond to an existing user, this method returns null.</returns>
	public virtual Task<IUIUser> GetUserAsync(string username)
	{
		return Task.FromResult<IUIUser>(null);
	}

	/// <summary>
	///       Updates the database with the information for the specified user.
	///       </summary>
	/// <param name="user">The user</param>
	/// <returns>The Task that represents the asynchronous operation, containing True if the operation succeeded otherwise False</returns>
	public virtual Task<UIUserActionResult> UpdateUserAsync(IUIUser user)
	{
		return Task.FromResult(new UIUserActionResult(status: true));
	}

	/// <summary>
	///        Deletes a user and any related user data.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <param name="deleteAllRelatedData">True to delete data related to the user from the database; false to leave data related to the user in the database.</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the user was deleted; otherwise, false.</returns>
	public virtual Task<bool> DeleteUserAsync(string username, bool deleteAllRelatedData)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///       Deletes a user and any related user data.
	///       </summary>
	/// <param name="providerName">The provider name</param>
	/// <param name="username">The user name</param>
	/// <param name="deleteAllRelatedData">True to delete data related to the user from the database; false to leave data related to the user in the database.</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the user was deleted; otherwise, false.</returns>
	public virtual Task<bool> DeleteUserAsync(string providerName, string username, bool deleteAllRelatedData)
	{
		return DeleteUserAsync(username, deleteAllRelatedData);
	}

	/// <summary>
	///       Get an instance of comparison object for IUserBase equality.
	///       </summary>
	/// <returns>An instance of comparison object for IUserBase equality.</returns>
	public virtual IEqualityComparer<IUIUser> GetUserComparer()
	{
		return _userComparer;
	}

	/// <summary>
	///        Asynchronously returns the number of elements in a sequence.
	///       </summary>
	/// <param name="predicate"> A function to test each element for a condition.</param>
	/// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the input sequence.</returns>
	public abstract Task<int> CountAsync(Expression<Func<IUIUser, bool>> predicate = null, CancellationToken cancellationToken = default(CancellationToken));

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///       Disposes the provider
	///       </summary>
	/// <param name="disposing">true if disposing</param>
	protected virtual void Dispose(bool disposing)
	{
	}
}

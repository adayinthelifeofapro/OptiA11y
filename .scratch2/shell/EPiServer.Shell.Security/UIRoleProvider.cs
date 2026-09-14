using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Security;

namespace EPiServer.Shell.Security;

/// <summary>
///        Manages roles for the users.
///       </summary>
public abstract class UIRoleProvider : IDisposable
{
	/// <summary>
	///        Gets or sets a value indicating whether role management is enabled.
	///       </summary>
	public abstract bool Enabled { get; set; }

	/// <summary>
	///       get the name of provider
	///       </summary>
	public abstract string Name { get; }

	/// <summary>
	///       Get the role provider if the provider aggregate providers then it can be used to resolve the provider.
	///       </summary>
	/// <param name="name">The name of provider</param>
	/// <returns>The provider name</returns>
	public virtual string GetRoleProviderName(string name)
	{
		return Name;
	}

	/// <summary>
	///        Determines whether a provider supports a specific EPiServer.Security.ProviderActions.
	///       </summary>
	/// <param name="action"> Action on provider EPiServer.Security.ProviderActions</param>
	/// <returns>Returns true if action on provider is supported otherwise false.</returns>
	public virtual bool IsSupported(ProviderActions action)
	{
		return false;
	}

	/// <summary>
	///       Gets all roles.
	///       </summary>
	/// <returns>Roles</returns>
	public abstract IAsyncEnumerable<IUIRole> GetAllRolesAsync();

	/// <summary>
	///        Creates a new role.
	///       </summary>
	/// <param name="newRoleName">The role name</param>
	public virtual Task<RoleResult> CreateRoleAsync(string newRoleName)
	{
		return Task.FromResult(RoleResult.Empty);
	}

	/// <summary>
	///        Gets a value indicating whether the specified role name already exists.
	///       </summary>
	/// <param name="roleName">The role name</param>
	public virtual Task<bool> RoleExistsAsync(string roleName)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Gets a list of users in the specified role.
	///       </summary>
	/// <param name="roleName">The role name</param>
	/// <returns>The Task that represents the asynchronous operation, containing a list of users that has the role name.</returns>
	public virtual IAsyncEnumerable<string> GetUsersInRoleAsync(string roleName)
	{
		return AsyncEnumerable.Empty<string>();
	}

	/// <summary>
	///        Removes the specified user from the specified role.
	///       </summary>
	/// <param name="userName">The user name</param>
	/// <param name="roleName">The role name</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual Task<RoleResult> RemoveUserFromRoleAsync(string userName, string roleName)
	{
		return Task.FromResult(RoleResult.Empty);
	}

	/// <summary>
	///       Gets a list of the roles that a user is in.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <returns>The Task that represents the asynchronous operation, containing a string array containing the names of all the roles that the specified user is in.</returns>
	public virtual IAsyncEnumerable<string> GetRolesForUserAsync(string username)
	{
		return AsyncEnumerable.Empty<string>();
	}

	/// <summary>
	///       Removes the specified user from the specified roles.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <param name="roleNames">The list of role name</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual Task<RoleResult> RemoveUserFromRolesAsync(string username, IEnumerable<string> roleNames)
	{
		return Task.FromResult(RoleResult.Empty);
	}

	/// <summary>
	///         Adds the specified user to the specified roles.
	///       </summary>
	/// <param name="username">The user name</param>
	/// <param name="roleNames">The role name</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual Task<RoleResult> AddUserToRolesAsync(string username, IEnumerable<string> roleNames)
	{
		return Task.FromResult(RoleResult.Empty);
	}

	/// <summary>
	///        Removes the specified user names from the specified roles.
	///       </summary>
	/// <param name="usernames">List of user names</param>
	/// <param name="roleNames">list of roles</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual Task<IEnumerable<RoleResult>> RemoveUsersFromRolesAsync(IEnumerable<string> usernames, IEnumerable<string> roleNames)
	{
		return Task.FromResult(Enumerable.Empty<RoleResult>());
	}

	/// <summary>
	///       Removes a role.
	///       </summary>
	/// <param name="roleName">The role name</param>
	/// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
	/// <returns> The Task that represents the asynchronous operation, containing true if the role was successfully deleted; otherwise, false.</returns>
	public virtual Task<bool> DeleteRoleAsync(string roleName, bool throwOnPopulatedRole)
	{
		return Task.FromResult(result: false);
	}

	/// <summary>
	///        Gets a list of users in the specified role for the configured applicationName.
	///       </summary>
	/// <param name="providerName">The name of provider</param>
	/// <param name="roleName">The role name</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual IAsyncEnumerable<string> GetUsersInRoleAsync(string providerName, string roleName)
	{
		return GetUsersInRoleAsync(roleName);
	}

	/// <summary>
	///       Removes the specified user names from the specified roles.
	///       </summary>
	/// <param name="providerName">The provider name</param>
	/// <param name="usernames">The list of user names</param>
	/// <param name="roleNames">The list of roles</param>
	/// <returns>The Task that represents the asynchronous operation.</returns>
	public virtual Task<IEnumerable<RoleResult>> RemoveUsersFromRolesAsync(string providerName, IEnumerable<string> usernames, IEnumerable<string> roleNames)
	{
		return RemoveUsersFromRolesAsync(usernames, roleNames);
	}

	/// <summary>
	///       Removes a role.
	///       </summary>
	/// <param name="providerName">The provider name</param>
	/// <param name="roleName">the role name</param>
	/// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
	/// <returns>The Task that represents the asynchronous operation, containing true if the role was successfully deleted; otherwise, false.</returns>
	public virtual Task<bool> DeleteRoleAsync(string providerName, string roleName, bool throwOnPopulatedRole)
	{
		return DeleteRoleAsync(roleName, throwOnPopulatedRole);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///       disposes the provider
	///       </summary>
	/// <param name="disposing">true if disposing</param>
	protected virtual void Dispose(bool disposing)
	{
	}
}

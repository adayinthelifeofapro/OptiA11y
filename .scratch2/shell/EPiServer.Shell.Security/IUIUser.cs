using System;

namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes and updates user information.
///       </summary>
public interface IUIUser
{
	/// <summary>
	///         Gets and sets the logon name of the user.
	///       </summary>
	string Username { get; set; }

	/// <summary>
	///       Gets or sets the e-mail address for the user.
	///       </summary>
	string Email { get; set; }

	/// <summary>
	///        Gets or sets whether the user can be authenticated.
	///       </summary>
	bool IsApproved { get; set; }

	/// <summary>
	///       Gets a value indicating whether the user is locked out and to be validated.
	///       </summary>
	bool IsLockedOut { get; set; }

	/// <summary>
	///        Gets the password question for the user.
	///       </summary>
	string PasswordQuestion { get; }

	/// <summary>
	///        Gets the name of the user provider that stores and retrieves user information for the  user.
	///       </summary>
	string ProviderName { get; }

	/// <summary>
	///        Gets or sets application-specific information for the user.
	///       </summary>
	string Comment { get; set; }

	/// <summary>
	///         Gets the date and time when the user was added to the user provider data store.
	///       </summary>
	DateTime CreationDate { get; }

	/// <summary>
	///         Gets or sets the date and time when the user was last authenticated.
	///       </summary>
	DateTime? LastLoginDate { get; set; }

	/// <summary>
	///        Gets the most recent date and time that the user was locked out.
	///       </summary>
	DateTime? LastLockoutDate { get; }
}

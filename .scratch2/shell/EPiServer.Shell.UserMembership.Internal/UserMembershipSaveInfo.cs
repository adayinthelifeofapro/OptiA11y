using System.Collections.Generic;

namespace EPiServer.Shell.UserMembership.Internal;

public class UserMembershipSaveInfo
{
	public string UserName { get; set; }

	public string Email { get; set; }

	public string PasswordQuestion { get; set; }

	public string PasswordAnswer { get; set; }

	public string CurrentPassword { get; set; }

	public string ConfirmPassword { get; set; }

	public bool IsAdminMode { get; set; }

	public string Password { get; set; }

	public bool ChangePassword { get; set; }

	public bool ChangeEmail { get; set; }

	public IEnumerable<string> Roles { get; set; }

	public bool IsApproved { get; set; }

	public bool IsLockedOut { get; set; }
}

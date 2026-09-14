using System;
using System.Collections.Generic;

namespace EPiServer.Shell.UserMembership.Internal;

public class UserMembershipInfo
{
	public string UserName { get; set; }

	public string Email { get; set; }

	public string Password { get; set; }

	public string PasswordAnswer { get; set; }

	public bool IsChangePassword { get; set; }

	public bool RequiresQuestionAndAnswer { get; set; }

	public string PasswordQuestion { get; set; }

	public bool IsApproved { get; set; }

	public bool IsLockedOut { get; set; }

	public IEnumerable<string> Roles { get; set; }

	public IEnumerable<string> AllRoles { get; set; }

	public DateTime? CreationDate { get; set; }

	public DateTime? LastLoginDate { get; set; }

	public string ProviderName { get; set; }

	public bool IsReadOnly { get; set; }
}

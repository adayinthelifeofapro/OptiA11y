using System;

namespace EPiServer.Shell.Security.Internal;

internal class UiUser : IUIUser
{
	public string Username { get; set; }

	public string Email { get; set; }

	public bool IsApproved { get; set; }

	public bool IsLockedOut { get; set; }

	public string PasswordQuestion { get; }

	public string ProviderName { get; }

	public string Comment { get; set; }

	public DateTime CreationDate { get; }

	public DateTime? LastLoginDate { get; set; }

	public DateTime? LastLockoutDate { get; }
}

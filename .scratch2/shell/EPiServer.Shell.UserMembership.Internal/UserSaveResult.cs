using System.Collections.Generic;
using EPiServer.Shell.Security;

namespace EPiServer.Shell.UserMembership.Internal;

public struct UserSaveResult
{
	public UIUserCreateStatus Status { get; set; }

	public IEnumerable<string> Errors { get; set; }
}

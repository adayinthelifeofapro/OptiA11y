using System;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Provides access to a newly created profile that can be modified before being sent to the client.
///       </summary>
public class ProfileEventArgs : EventArgs
{
	/// <summary>
	///       A newly created profile.
	///       </summary>
	public ProfileData Profile { get; set; }
}

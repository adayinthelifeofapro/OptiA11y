using System;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Provides access to a user profile.
///       </summary>
public interface IProfileRepository
{
	/// <summary>
	///       Invoked when a profile is created for the first time.
	///       </summary>
	event EventHandler<ProfileEventArgs> ProfileCreated;

	/// <summary>
	///       Gets an existing user profile.
	///       </summary>
	/// <param name="userName">The username of the profile to get.</param>
	/// <returns>An existing profile or null if no profile exists.</returns>
	ProfileData GetProfile(string userName);

	/// <summary>
	///       Gets or creates a user profile.
	///       </summary>
	/// <param name="userName">The user name of the profile to get or create.</param>
	/// <returns>An existing user profile, or a newly created profile if no profile previously existed.</returns>
	ProfileData GetOrCreateProfile(string userName);

	/// <summary>
	///       Saves the given profile to persistence medium.
	///       </summary>
	/// <param name="profile">The profile data to store.</param>
	void Save(ProfileData profile);

	/// <summary>
	///       Deletes the profile associated with a user name.
	///       </summary>
	/// <param name="userName">The user name of the profile to delete.</param>
	void Delete(string userName);
}

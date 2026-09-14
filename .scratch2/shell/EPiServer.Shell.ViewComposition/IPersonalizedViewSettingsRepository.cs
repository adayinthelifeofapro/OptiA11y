using System.Collections.Generic;
using System.Security.Principal;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
/// </summary>
public interface IPersonalizedViewSettingsRepository
{
	/// <summary>
	///       Load settings for a specific view owned by the supplied user..
	///       </summary>
	/// <param name="principal">The principal to load settings for.</param>
	/// <param name="viewName">Name of the view to load settings for.</param>
	/// <returns>Settings for a specific view</returns>
	PersonalizedViewSettings Load(IPrincipal principal, string viewName);

	/// <summary>
	///       Loads all settings owned by a specific user.
	///       </summary>
	/// <param name="principal">The principal to load settings for.</param>
	/// <returns>A collection of settings for the supplied principal</returns>
	IEnumerable<PersonalizedViewSettings> Load(IPrincipal principal);

	/// <summary>
	///       Save the specified settings to the backing store.
	///       </summary>
	/// <param name="settings">The settings to persist.</param>
	void Save(PersonalizedViewSettings settings);

	/// <summary>
	///       Deletes the settings object for the specified user and view thus restoring the view settings to system default.
	///       </summary>
	/// <param name="principal">The principal to load settings for.</param>
	/// <param name="viewName">Name of the view to load settings for.</param>
	void Delete(IPrincipal principal, string viewName);
}

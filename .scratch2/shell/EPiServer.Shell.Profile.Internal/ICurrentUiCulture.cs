using System.Globalization;

namespace EPiServer.Shell.Profile.Internal;

/// <summary>
///       UI Language service used resolve the preferred ui culture for a user
///       </summary>
public interface ICurrentUiCulture
{
	/// <summary>
	///       Returns the preferred UI Culture for the user
	///       </summary>
	/// <param name="userName">The userName for the user to get the ui culture for</param>
	/// <returns>
	/// </returns>
	CultureInfo Get(string userName);

	/// <summary>
	///       Saves the preferred ui culture for the user
	///       </summary>
	/// <param name="userName">The userName for the user to store the ui culture</param>
	/// <param name="culture">The preferred ui culture</param>
	void Save(string userName, CultureInfo culture);

	/// <summary>
	///       Returns the default UI Culture for the application
	///       </summary>
	/// <returns>
	/// </returns>
	CultureInfo GetDefault();
}

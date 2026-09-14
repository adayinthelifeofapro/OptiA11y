using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Licensing;

/// <summary>
///       Specifies the path to the license file
///       </summary>
[Options(ConfigurationSection = "Cms")]
public class LicensePathOptions
{
	/// <summary>
	///       The path to the license file
	///       </summary>
	/// <remarks>
	///   <para>
	///       If path is non rooted it is interpreted as relative the application
	///       </para>
	///   <para>
	///       Default value is <see cref="F:EPiServer.Licensing.LicensingOptions.DefaultLicenseFile" /></para>
	/// </remarks>
	public string Path { get; set; }
}

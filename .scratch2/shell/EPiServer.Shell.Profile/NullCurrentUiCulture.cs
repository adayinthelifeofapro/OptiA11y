using System.Globalization;
using EPiServer.Shell.Profile.Internal;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Default null implementation of the ICurrentUiCulture interface
///       This class only exists to solve problem in Mirroring when the Shell dll is copied to the mirroring instance but not CMS
///       </summary>
internal class NullCurrentUiCulture : ICurrentUiCulture
{
	public CultureInfo Get(string userName)
	{
		return CultureInfo.InvariantCulture;
	}

	public CultureInfo GetDefault()
	{
		return CultureInfo.InvariantCulture;
	}

	public void Save(string userName, CultureInfo culture)
	{
	}
}

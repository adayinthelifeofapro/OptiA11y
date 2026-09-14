using EPiServer.Licensing;
using Microsoft.Extensions.Options;

namespace EPiServer.Shell.Licensing;

internal class LicensingOptionsConfigurer : IConfigureOptions<LicensingOptions>
{
	private readonly LicensePathOptions _licensePathOptions;

	public LicensingOptionsConfigurer(LicensePathOptions licensePathOptions)
	{
		_licensePathOptions = licensePathOptions;
	}

	public void Configure(LicensingOptions options)
	{
		if (!string.IsNullOrEmpty(_licensePathOptions.Path))
		{
			options.LicenseFilePath = _licensePathOptions.Path;
		}
	}
}

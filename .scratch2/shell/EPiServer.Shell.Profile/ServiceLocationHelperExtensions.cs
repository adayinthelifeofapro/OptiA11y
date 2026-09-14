using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Makes the profile part of the public API.
///       </summary>
public static class ServiceLocationHelperExtensions
{
	/// <summary>
	///       Resolves the <see cref="T:EPiServer.Shell.Profile.IProfileRepository" />.
	///       </summary>
	/// <param name="helper">The service helper.</param>
	/// <returns>The currently registered <see cref="T:EPiServer.Shell.Profile.IProfileRepository" />.</returns>
	[Obsolete("Retrieve the IProfileRepository service using constructor injection or retrieve it from the service provider.")]
	public static IProfileRepository ProfileRepository(this ServiceProviderHelper helper)
	{
		return ServiceProviderExtensions.GetInstance<IProfileRepository>(helper.Advanced);
	}
}

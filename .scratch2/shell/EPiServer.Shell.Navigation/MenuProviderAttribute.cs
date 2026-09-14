using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Menu provider attribute. A wrapper for the MEF's Export attribute for IMenuProvider interface.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class MenuProviderAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Gets <see cref="F:EPiServer.ServiceLocation.ServiceInstanceScope.Singleton" />.
	///       </summary>
	public override ServiceInstanceScope Lifecycle => (ServiceInstanceScope)0;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuProviderAttribute" /> class.
	///       </summary>
	public MenuProviderAttribute()
		: base(typeof(IMenuProvider))
	{
	}
}

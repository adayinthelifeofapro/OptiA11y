using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Attribute for the shell modules providers
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ModuleProviderAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Gets <see cref="F:EPiServer.ServiceLocation.ServiceInstanceScope.Singleton" />.
	///       </summary>
	public override ServiceInstanceScope Lifecycle => (ServiceInstanceScope)0;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleProviderAttribute" /> class.
	///       </summary>
	public ModuleProviderAttribute()
		: base(typeof(IModuleProvider))
	{
	}
}

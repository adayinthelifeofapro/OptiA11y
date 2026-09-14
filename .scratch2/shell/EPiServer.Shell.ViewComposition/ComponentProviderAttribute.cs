using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" /> for the system. The type using this attribute
///       must implement <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentProviderAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Shell.ViewComposition.ComponentProviderAttribute" />.
	///       </summary>
	public ComponentProviderAttribute()
		: base(typeof(IComponentProvider))
	{
	}
}

using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a <see cref="T:EPiServer.Shell.ViewComposition.IViewProvider" /> for the system. The type using this attribute
///       must implement <see cref="T:EPiServer.Shell.ViewComposition.IViewProvider" />.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewProviderAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Shell.ViewComposition.ViewProviderAttribute" />.
	///       </summary>
	public ViewProviderAttribute()
		: base(typeof(IViewProvider))
	{
	}
}

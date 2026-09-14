using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" /> for the system. The type using this attribute
///       must implement <see cref="T:EPiServer.Shell.ViewComposition.ICompositeView" />.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class CompositeViewAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Shell.ViewComposition.CompositeViewAttribute" />.
	///       </summary>
	public CompositeViewAttribute()
		: base(typeof(ICompositeView))
	{
	}
}

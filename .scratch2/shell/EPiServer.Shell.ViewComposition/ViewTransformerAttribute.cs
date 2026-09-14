using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" /> for the system. The type using this attribute
///       must implement <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" />.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewTransformerAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Shell.ViewComposition.ViewTransformerAttribute" />.
	///       </summary>
	public ViewTransformerAttribute()
		: base(typeof(IViewTransformer))
	{
	}
}

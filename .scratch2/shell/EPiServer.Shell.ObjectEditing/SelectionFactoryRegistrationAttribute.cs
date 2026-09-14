using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Register a <see cref="T:EPiServer.Shell.ObjectEditing.ISelectionFactory" /> instance to IOC container.
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SelectionFactoryRegistrationAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.SelectionFactoryRegistrationAttribute" /> class.
	///       </summary>
	public SelectionFactoryRegistrationAttribute()
		: base(typeof(ISelectionFactory))
	{
	}
}

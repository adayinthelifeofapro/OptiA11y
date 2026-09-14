using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell;

/// <summary>
///       Represents attributes configuring plugin services for registering ContentUIDescriptor
///       </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class UIDescriptorRegistrationAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptorRegistrationAttribute" /> class.
	///       </summary>
	public UIDescriptorRegistrationAttribute()
		: base(typeof(UIDescriptor))
	{
	}
}

using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ModelAccessorCreatorRegistrationAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Gets or sets the editor descriptor type that you want to use.
	///       </summary>
	/// <value>The editor descriptor type.</value>
	public Type TargetType { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.ModelAccessorCreatorRegistrationAttribute" /> class.
	///       </summary>
	public ModelAccessorCreatorRegistrationAttribute()
		: base(typeof(IModelAccessorCreator))
	{
	}
}

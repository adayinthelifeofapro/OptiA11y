namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Specifies how an editor descriptor should interact with other descriptors for the same type.
///       </summary>
public enum EditorDescriptorBehavior
{
	/// <summary>
	///       Adds this descriptor to the list of descriptors for the given type and ui hint combination.
	///       </summary>
	Default,
	/// <summary>
	///       Adds this descriptor to the list of descriptors for the given type and ui hint combination and
	///       makes sure that the descriptors registered without a ui hint for the type are called before this descriptor.
	///       </summary>
	/// <remarks>This is only valid in combination with a ui hint.</remarks>
	ExtendBase,
	/// <summary>
	///       Removes any existing descriptors for the type/ui hint combination and then adds this descriptor.
	///       </summary>
	OverrideDefault,
	/// <summary>
	///       Adds this descriptor last in the list of descriptors for the given type and ui hint combination.
	///       </summary>
	/// <remarks>If several descriptors are defined this way the order of execution is undefined.</remarks>
	PlaceLast
}

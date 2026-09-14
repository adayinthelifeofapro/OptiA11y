namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines the type of a container.
///       </summary>
public enum ContainerType
{
	/// <summary>
	///       Not defined.
	///       </summary>
	None,
	/// <summary>
	///       A system container is composed by the system and a user may not add components.
	///       </summary>
	System,
	/// <summary>
	///       A user container can be modified by the user.
	///       </summary>
	User
}

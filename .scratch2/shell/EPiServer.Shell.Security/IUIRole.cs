namespace EPiServer.Shell.Security;

/// <summary>
///        Exposes and updates role information.
///       </summary>
public interface IUIRole
{
	/// <summary>
	///         Gets the role name.
	///       </summary>
	string Name { get; set; }

	/// <summary>
	///        Gets the name of role.
	///       </summary>
	string ProviderName { get; set; }
}

namespace EPiServer.Shell.Modules;

/// <summary>
///       Used to transfer <see cref="T:EPiServer.Shell.Configuration.ModuleDependency" />  into a format that is suitable for client consumption.
///       </summary>
public class ModuleDependencyViewModel
{
	/// <summary>
	///       Gets or sets the name of the module.
	///       </summary>
	/// <value>
	///       The name of the module.
	///       </value>
	public string ModuleName { get; set; }

	/// <summary>
	///       Gets or sets the type of the dependency.
	///       </summary>
	/// <value>
	///       The type of the dependency.
	///       </value>
	public int DependencyType { get; set; }
}

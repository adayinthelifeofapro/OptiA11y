using System.Collections.Generic;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       An abstract base class for <see cref="T:EPiServer.Shell.Modules.PublicModuleOptions" /> and <see cref="T:EPiServer.Shell.Modules.ProtectedModuleOptions" /> to be used
///       in the same way when needed.
///       </summary>
public abstract class ModuleOptionsBase
{
	/// <summary>
	///       The root path below which module directories are located.
	///       </summary>
	public string RootPath { get; set; }

	/// <summary>
	///       Option for probing the the module folder and load of module assemblies automatically on start-up.
	///       </summary>
	public AutoDiscoveryLevel AutoDiscovery { get; set; } = AutoDiscoveryLevel.Minimal;

	/// <summary>
	///       Gets all the modules item for the current instance of module
	///       </summary>
	public IList<ModuleDetails> Items { get; } = new List<ModuleDetails>();
}

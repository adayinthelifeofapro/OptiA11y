using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Provides modules in the shell module system.
///       </summary>
public interface IModuleProvider
{
	/// <summary>
	///       Gets the modules.
	///       </summary>
	/// <returns>An enumeration of modules</returns>
	IEnumerable<ShellModule> GetModules();
}

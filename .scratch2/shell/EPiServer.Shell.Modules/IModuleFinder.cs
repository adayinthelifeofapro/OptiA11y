using System.Collections.Generic;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Discovers modules in a folder structure and registers them into the application
///       </summary>
/// <remarks>This class is used internally by EPiServer and should not be instantiated.</remarks>
public interface IModuleFinder
{
	/// <summary>
	///       Get modules from sub-directories of the supplied directory.
	///       </summary>
	/// <param name="rootPath">The root folder for modules to discover.</param>
	/// <param name="discoveryMode">The level to use for the auto discovery.</param>
	/// <returns>
	///       A list of shell modules
	///       </returns>
	IList<ShellModule> GetModulesInSubdirectories(string rootPath, AutoDiscoveryLevel discoveryMode);

	/// <summary>
	///       Extracts and loads a module in a directory
	///       </summary>
	/// <param name="routeBasePath">The base path for the routes.</param>
	/// <param name="moduleResourcePath">The virtual path to module resources</param>
	/// <param name="configuredAssemblyNames">Assemblies that are always loaded and associated with the module.</param>
	/// <param name="discoveryMode">What level the auto discovery should be.</param>
	/// <param name="configuredName">Configured module name.</param>
	/// <returns>
	///       A shell module.
	///       </returns>
	ShellModule GetModuleInDirectory(string routeBasePath, string moduleResourcePath, IEnumerable<string> configuredAssemblyNames, AutoDiscoveryLevel discoveryMode, string configuredName);
}

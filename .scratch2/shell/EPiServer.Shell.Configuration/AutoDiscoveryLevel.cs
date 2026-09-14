namespace EPiServer.Shell.Configuration;

/// <summary>
///       Option for auto-discovery of modules
///       </summary>
public enum AutoDiscoveryLevel
{
	/// <summary>
	///       The most secure option. Will only load modules configured by web.config 
	///       and associate assemblies explicitly configured in web.config
	///       </summary>
	Minimal = 1,
	/// <summary>
	///       Auto-discover modules in the module directory and load assemblies defined in the 
	///       module's module.config file and load assemblies located in the module's bin 
	///       directory in addition to those defined in the module's module.config file
	///       </summary>
	Modules
}

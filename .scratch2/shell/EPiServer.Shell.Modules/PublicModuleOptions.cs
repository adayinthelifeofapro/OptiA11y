using EPiServer.ServiceLocation;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules.Internal;

namespace EPiServer.Shell.Modules;

/// <summary>
///       A collection of publicly accessible modules. In addition to modules
///       explicitly configured in this collection modules will also be auto-discovered
///       from the resource root path.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class PublicModuleOptions : ModuleOptionsBase
{
	public PublicModuleOptions()
	{
		base.RootPath = "~/modules/";
		base.AutoDiscovery = AutoDiscoveryLevel.Modules;
	}
}

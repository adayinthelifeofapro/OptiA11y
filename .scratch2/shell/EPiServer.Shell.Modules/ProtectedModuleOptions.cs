using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules.Internal;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Represents a collection of modules only accessible for authenticated users.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class ProtectedModuleOptions : ModuleOptionsBase
{
	public ProtectedModuleOptions()
	{
		base.RootPath = "~/Optimizely/";
		base.Items.Add(new ModuleDetails
		{
			Name = "Shell"
		});
		base.Items.Add(new ModuleDetails
		{
			Name = "CMS"
		});
		base.Items.Add(new ModuleDetails
		{
			Name = "Settings"
		});
		base.Items.Add(new ModuleDetails
		{
			Name = "Profile"
		});
	}
}

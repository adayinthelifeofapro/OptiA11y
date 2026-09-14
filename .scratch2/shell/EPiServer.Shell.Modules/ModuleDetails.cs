using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Represents the details of the module inside episerver.shell.protectedModules or publicModules configuration.
///       </summary>
public class ModuleDetails
{
	/// <summary>
	///       The name of the module. This is used to find the module directory.
	///       </summary>
	public string Name { get; set; }

	/// <summary>
	///       The path to the module. This is used to find the module directory. Default value is {rootpath}{modulename}
	///       </summary>
	public string ResourcePath { get; set; } = "{rootpath}{modulename}";

	/// <summary>
	///       Client resource path. If not specified the <see cref="P:EPiServer.Shell.Modules.ModuleDetails.ResourcePath" /> is used.
	///       </summary>
	public string ClientResourcePath { get; set; }

	/// <summary>
	///       Full Assembly names to load and associate with the module. This value may be combined with assemblies defined by the module depending on the auto discovery option.
	///       </summary>
	public IList<string> Assemblies { get; } = new List<string>();
}

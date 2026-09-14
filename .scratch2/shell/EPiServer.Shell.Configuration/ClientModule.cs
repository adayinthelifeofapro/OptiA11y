using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines client resources and dependencies of a shell module. 
///       </summary>
[Serializable]
public class ClientModule
{
	/// <summary>
	///       Gets or sets the initializer class used to start a module on the client.
	///       </summary>
	/// <value>The initializer.</value>
	[XmlAttribute("initializer")]
	public string Initializer { get; set; }

	/// <summary>
	///       Gets or sets the module dependencies.
	///       </summary>
	/// <value>The module dependencies.</value>
	[XmlArray("moduleDependencies")]
	[XmlArrayItem("add")]
	public List<ModuleDependency> ModuleDependencies { get; set; }

	/// <summary>
	///       Gets or sets the list of client resources that should be loaded for this client module
	///       </summary>
	/// <value>The resources.</value>
	[XmlArray("requiredResources")]
	[XmlArrayItem("add")]
	public List<ClientResourceReference> RequiredResources { get; set; }
}

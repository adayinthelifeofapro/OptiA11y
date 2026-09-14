using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Client-side shell module dependency definition
///       </summary>
[Serializable]
public class ModuleDependency
{
	/// <summary>
	///       Name of shell module that there is a dependency on
	///       </summary>
	[XmlAttribute("dependency")]
	public string Dependency { get; set; }

	/// <summary>
	///       Module dependency type
	///       </summary>
	[XmlAttribute("type")]
	public ModuleDependencyTypes DependencyType { get; set; }
}

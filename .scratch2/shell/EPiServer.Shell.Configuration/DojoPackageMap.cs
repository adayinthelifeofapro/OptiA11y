using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines a package map
///       </summary>
[Serializable]
public class DojoPackageMap
{
	/// <summary>
	///       The name of the packageMap as referenced from <see cref="T:EPiServer.Shell.Configuration.DojoPackage" /> items
	///       </summary>
	[XmlAttribute(AttributeName = "name")]
	public string Name { get; set; }

	/// <summary>
	///       The mappings that make out the <see cref="T:EPiServer.Shell.Configuration.DojoPackageMap" /></summary>
	[XmlArray("mappings")]
	[XmlArrayItem("add")]
	public List<DojoPackageMapping> Mappings { get; set; }
}

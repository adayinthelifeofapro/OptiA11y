using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       The dojo configuration provided by a shell module. 
///       </summary>
[Serializable]
public class DojoConfiguration
{
	/// <summary>
	///       Gets or sets the dojo aliases
	///       </summary>
	[XmlArray("aliases")]
	[XmlArrayItem("add")]
	public List<DojoAlias> Aliases { get; set; }

	/// <summary>
	///       Gets or sets the dojo paths
	///       </summary>
	[XmlArray("paths")]
	[XmlArrayItem("add")]
	public List<DojoPath> Paths { get; set; }

	/// <summary>
	///       Gets or sets the dojo packages
	///       </summary>
	[XmlArray("packages")]
	[XmlArrayItem("add")]
	public List<DojoPackage> Packages { get; set; }

	/// <summary>
	///       Gets or sets the dojo maps
	///       </summary>
	[XmlArray("packageMaps")]
	[XmlArrayItem("add")]
	public List<DojoPackageMap> PackageMaps { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.DojoConfiguration" /> class.
	///       </summary>
	public DojoConfiguration()
	{
		Aliases = new List<DojoAlias>();
		Paths = new List<DojoPath>();
		Packages = new List<DojoPackage>();
		PackageMaps = new List<DojoPackageMap>();
	}
}

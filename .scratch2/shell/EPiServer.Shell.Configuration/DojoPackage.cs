using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines a package
///       </summary>
/// <example>
///       Name: "epi-myPackage"
///       Location: "//myServer/epi-myPackage"
///       Main: 
///       </example>
[Serializable]
public class DojoPackage
{
	/// <summary>
	///       The name of the package
	///       </summary>
	[XmlAttribute(AttributeName = "name")]
	public string Name { get; set; }

	/// <summary>
	///       The location of the package, can either be a path relative to baseUrl or an absolute path
	///       </summary>
	[XmlAttribute(AttributeName = "location")]
	public string Location { get; set; }

	/// <summary>
	///       Optional parameter for indicating the module to load when package is required; defaults to "main" if not specified
	///       </summary>
	[XmlAttribute(AttributeName = "main")]
	public string Main { get; set; }

	/// <summary>
	///       References the Name attribute of the <see cref="T:EPiServer.Shell.Configuration.DojoPackageMap" /> to use to transparently remap package references
	///       </summary>
	[XmlAttribute(AttributeName = "packageMap")]
	public string PackageMap { get; set; }
}

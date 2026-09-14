using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines a package mapping
///       </summary>
[Serializable]
public class DojoPackageMapping
{
	/// <summary>
	///       Gets or sets original package name
	///       </summary>
	[XmlAttribute(AttributeName = "key")]
	public string Key { get; set; }

	/// <summary>
	///       Gets or sets the name of package to map to
	///       </summary>
	[XmlAttribute(AttributeName = "value")]
	public string Value { get; set; }
}

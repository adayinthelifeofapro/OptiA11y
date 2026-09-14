using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       A serializable assembly name container
///       </summary>
public class AssemblyElement
{
	/// <summary>
	///       An assembly name.
	///       </summary>
	[XmlAttribute("assembly")]
	public string Assembly { get; set; }

	/// <summary>
	///       This attribute helps migration from previous versions, do not use.
	///       </summary>
	[XmlAttribute("name")]
	public string Name { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.AssemblyElement" /> class.
	///       </summary>
	public AssemblyElement()
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.AssemblyElement" /> class.
	///       </summary>
	/// <param name="value">The name of something.</param>
	public AssemblyElement(string value)
	{
		Assembly = value;
	}
}

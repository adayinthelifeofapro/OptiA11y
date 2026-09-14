using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines a dojo path as provided by a shell module. 
///       </summary>
[Serializable]
public class DojoPath
{
	private string _path;

	/// <summary>
	///       Name of the dojo module
	///       </summary>
	[XmlAttribute("name")]
	public string Name { get; set; }

	/// <summary>
	///       Path to the module root. Defaults to Scripts/[Name]
	///       </summary>
	[XmlAttribute("path")]
	public string Path
	{
		get
		{
			return _path ?? ("Scripts/" + Name.Replace('.', '/'));
		}
		set
		{
			_path = value;
		}
	}
}

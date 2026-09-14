using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Reference to client-side resource
///       </summary>
[Serializable]
public class ClientResourceReference
{
	/// <summary>
	///       Client resource name
	///       </summary>
	/// <seealso cref="T:EPiServer.Framework.Web.Resources.ClientResource" />
	[XmlAttribute("name")]
	public string Name { get; set; }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Meta-data for a module. The manifest is parsed from the file module.config in the module's
///       root directory.
///       </summary>
[Serializable]
[XmlRoot("module")]
public class ShellModuleManifest
{
	/// <summary>
	///       Help file for the module
	///       </summary>
	[XmlAttribute("helpFile")]
	public string HelpFile { get; set; }

	/// <summary>
	///       Product name
	///       </summary>
	[XmlAttribute("productName")]
	public string ProductName { get; set; }

	/// <summary>
	///       Gets or sets the client resource path relative to base resource module folder.
	///       </summary>
	/// <value>
	///       The relative client resource path.
	///       </value>
	[XmlAttribute("clientResourceRelativePath")]
	public string ClientResourceRelativePath { get; set; }

	/// <summary>
	///       Gets or sets the custom type for this module.
	///       </summary>
	/// <value>The custom type of the module.</value>
	/// <remarks>If not set, <see cref="T:EPiServer.Shell.Modules.ShellModule" /> will be used.</remarks>
	[XmlAttribute("type")]
	public string Type { get; set; }

	/// <summary>
	///       A list of assemblies where modules will be loaded from.
	///       </summary>
	[XmlArray("assemblies")]
	[XmlArrayItem("add")]
	public List<AssemblyElement> Assemblies { get; set; }

	/// <summary>
	///       Gets or sets the route base path.
	///       </summary>
	/// <value>
	///       The route base path.
	///       </value>
	[XmlAttribute("routeBasePath")]
	public string RouteBasePath { get; set; }

	/// <summary>
	///       A definition of the routes for modules.
	///       </summary>
	/// <remarks>Route is a quick entry to the first entry of the <see cref="P:EPiServer.Shell.Configuration.ShellModuleManifest.Routes">Routes</see> collection.</remarks>
	[XmlElement("route")]
	public RouteDescription Route
	{
		get
		{
			return Routes.FirstOrDefault();
		}
		set
		{
			Routes.Insert(0, value);
		}
	}

	/// <summary>
	///       A list of routes for the module.
	///       </summary>
	[XmlArray("routes")]
	[XmlArrayItem("route")]
	public List<RouteDescription> Routes { get; set; }

	/// <summary>
	///       Indicates whether the application will look in all bin folders for modules.
	///       </summary>
	[XmlAttribute("loadFromBin")]
	public bool LoadFromBin { get; set; }

	/// <summary>
	///       Gets or sets the version for the shell module.
	///       </summary>
	[XmlAttribute("version")]
	public string Version { get; set; }

	/// <summary>
	///       Gets or sets the client resources provided by the module.
	///       </summary>
	[XmlArray("clientResources")]
	[XmlArrayItem("add")]
	public List<ClientResourceElement> ClientResources { get; set; }

	/// <summary>
	///       Gets or sets the dojo modules provided by the shell module
	///       </summary>
	/// <remarks>This is obsolete, use Dojo.Paths instead.</remarks>
	[XmlArray("dojoModules")]
	[XmlArrayItem("add")]
	public List<DojoPath> DojoModules { get; set; }

	/// <summary>
	///       Gets or sets the client module settings.
	///       </summary>
	[XmlElement("clientModule")]
	public ClientModule ClientModule { get; set; }

	/// <summary>
	///       Gets or sets the dojo configuration
	///       </summary>
	[XmlElement("dojo")]
	public DojoConfiguration Dojo { get; set; }

	/// <summary>
	///       Gets or sets the Name of the folder that contains razor views
	///       </summary>
	/// <remarks>If not set then the expected location is &lt;ModuleNamegt;.Views</remarks>
	[XmlAttribute("viewFolder")]
	public string ViewFolder { get; set; }

	/// <summary>
	///       Gets or sets the authorization policy for the shell module.
	///       </summary>
	[XmlAttribute("authorizationPolicy")]
	public string AuthorizationPolicy { get; set; }

	/// <summary>
	///       Gets or sets the client resources authorization policy for the shell module.
	///       </summary>
	[XmlAttribute("clientAuthorizationPolicy")]
	public string ClientAuthorizationPolicy { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.ShellModuleManifest" /> class.
	///       Default value for LoadFromBin is true.
	///       </summary>
	public ShellModuleManifest()
	{
		Assemblies = new List<AssemblyElement>();
		ClientResources = new List<ClientResourceElement>();
		DojoModules = new List<DojoPath>();
		Routes = new List<RouteDescription>();
		LoadFromBin = true;
		Dojo = new DojoConfiguration();
	}

	/// <summary>
	///       Deserializes the stream into a ShellModuleManifest.
	///       </summary>
	/// <param name="manifestFileStream">A stream containing the object serialized as xml.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Configuration.ShellModuleManifest" /></returns>
	public static ShellModuleManifest Deserialize(Stream manifestFileStream)
	{
		return new XmlSerializer(typeof(ShellModuleManifest)).Deserialize(manifestFileStream) as ShellModuleManifest;
	}
}

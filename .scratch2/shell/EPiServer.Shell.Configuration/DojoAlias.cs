using System;
using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Defines an alias that maps one dojo module to another
///       </summary>
/// <example>
///       From: "epi/cms/widgets/ColorPicker"
///       To: "epi-cms/widgets/NewAdvancedPicker"
///       </example>
[Serializable]
public class DojoAlias
{
	/// <summary>
	///       The name of the module to alias
	///       </summary>
	[XmlAttribute(AttributeName = "from")]
	public string From { get; set; }

	/// <summary>
	///       The new module name
	///       </summary>
	[XmlAttribute(AttributeName = "to")]
	public string To { get; set; }
}

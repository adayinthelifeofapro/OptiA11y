using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Web;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Attribute to overwrite default list item header to a string based on custom property values
///       </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ListItemHeaderPropertyAttribute : Attribute
{
	/// <summary>
	///       Textual property names which will be used to create a custom list item header
	///       </summary>
	public IEnumerable<string> Properties { get; }

	/// <summary>
	///       Create a new instance of ListItemHeaderAttribute
	///       </summary>
	/// <param name="properties">Textual property names which will be used to create a custom list item header</param>
	public ListItemHeaderPropertyAttribute(params string[] properties)
	{
		Properties = properties.Select((string x) => x.NameCamelCase());
	}
}

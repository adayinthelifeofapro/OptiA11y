using System;
using System.ComponentModel.DataAnnotations;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Attribute to overwrite system's default UI Hint for a generic list item type
///       </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ListItemUIHintAttribute : UIHintAttribute
{
	public ListItemUIHintAttribute(string uiHint)
		: base(uiHint)
	{
	}

	public ListItemUIHintAttribute(string uiHint, string presentationLayer)
		: base(uiHint, presentationLayer)
	{
	}

	public ListItemUIHintAttribute(string uiHint, string presentationLayer, params object[] controlParameters)
		: base(uiHint, presentationLayer, controlParameters)
	{
	}
}

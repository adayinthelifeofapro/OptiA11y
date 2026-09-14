using System;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Used to define group settings for a property group on a class.
///       </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class GroupSettingsAttribute : Attribute
{
	/// <summary>
	///       Gets or sets the localized title.
	///       </summary>
	/// <value>The title.</value>
	public string Title { get; set; }

	/// <summary>
	///       Gets or sets the name that is used as an identifier for the group.
	///       </summary>
	/// <value>The name.</value>
	public string Name { get; set; }

	/// <summary>
	///       Gets or sets the client class used to layout the property group, usually a DOJO widget.
	///       </summary>
	/// <value>The client side layout class.</value>
	public string ClientLayoutClass { get; set; }
}

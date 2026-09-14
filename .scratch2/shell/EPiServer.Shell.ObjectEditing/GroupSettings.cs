using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Provides information on how property groups should work. This can be used to sort and display property groups.
///       </summary>
public class GroupSettings
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
	///       States if a group will be shown on the UI.
	///       </summary>
	public bool DisplayUI { get; set; }

	/// <summary>
	///       Gets or sets the display order.
	///       </summary>
	/// <value>
	///       The display order.
	///       </value>
	public int DisplayOrder { get; set; }

	/// <summary>
	///       Gets or sets the client class used to layout the property group, usually a DOJO widget.
	///       </summary>
	/// <value>The client side layout class.</value>
	public string ClientLayoutClass { get; set; }

	/// <summary>
	///       Gets or sets the options.
	///       </summary>
	/// <value>
	///       The options.
	///       </value>
	public IDictionary<string, object> Options { get; private set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.GroupSettings" /> class.
	///       </summary>
	public GroupSettings()
	{
		Options = new Dictionary<string, object>();
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.GroupSettings" /> class.
	///       </summary>
	/// <param name="name">Name that is used as an identifier for the group.</param>
	/// <param name="displayUI">States if a group will be shown on the UI.</param>
	/// <param name="clientLayoutClass">Client class used to layout the property group, usually a DOJO widget.</param>
	/// <param name="options">The options.</param>
	public GroupSettings(string name, bool displayUI, string clientLayoutClass, IDictionary<string, object> options)
		: this()
	{
		Name = name;
		DisplayUI = displayUI;
		ClientLayoutClass = clientLayoutClass;
		Options = options;
	}
}

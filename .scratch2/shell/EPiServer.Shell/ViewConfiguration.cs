using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EPiServer.Framework;

namespace EPiServer.Shell;

/// <summary>
///       Class to define configuration of sparrowhawk(R) view for specific type.
///       </summary>
public abstract class ViewConfiguration<T> : ViewConfiguration
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewConfiguration`1" /> class.
	///       </summary>
	protected ViewConfiguration()
		: base(typeof(T))
	{
		base.SortOrder = 20000;
	}
}
/// <summary>
///       Class to define configuration of sparrowhawk(R) view for specific type.
///       </summary>
public abstract class ViewConfiguration : ISortable
{
	public class ViewFeatures
	{
		public bool DeviceSelection { get; set; }

		public bool VisitorGroups { get; set; }

		public bool IframeSupported { get; set; } = true;
	}

	private Type _forType;

	/// <summary>
	///       Gets or sets for type.
	///       </summary>
	/// <value>
	///       For type.
	///       </value>
	[IgnoreDataMember]
	[JsonIgnore]
	public virtual Type ForType
	{
		get
		{
			return _forType;
		}
		set
		{
			_forType = value;
		}
	}

	/// <summary>
	///       Gets the key that's used to find this view configuration.
	///       </summary>
	/// <value>
	///       The key.
	///       </value>
	public string Key { get; set; }

	/// <summary>
	///       Gets or sets the name.
	///       </summary>
	/// <value>
	///       The name.
	///       </value>
	public string Name { get; set; }

	/// <summary>
	///       Gets or sets the description.
	///       </summary>
	/// <value>
	///       The description.
	///       </value>
	public string Description { get; set; }

	/// <summary>
	///       Gets or sets the language path.
	///       </summary>
	/// <value>
	///       The name.
	///       </value>
	public string LanguagePath { get; set; }

	/// <summary>
	///       Gets or sets the icon class.
	///       </summary>
	/// <value>
	///       The icon class.
	///       </value>
	public string IconClass { get; set; }

	/// <summary>
	///       Gets or sets the show on view menu.
	///       </summary>
	/// <value>
	///       The show on view menu.
	///       </value>
	public virtual bool HideFromViewMenu { get; set; }

	/// <summary>
	///       Gets the full identifier for the widget used to represent the view, for instance "mynamespace/widget/widgettype".
	///       </summary>
	/// <value>
	///       The type of the view.
	///       </value>
	public string ViewType { get; set; }

	/// <summary>
	///       Gets the full identifier for the controller used to represent the view, for instance "mynamespace/widget/widgettype".
	///       </summary>
	/// <value>
	///       The type of the controller.
	///       </value>
	public string ControllerType { get; set; }

	/// <summary>
	///       Used to sort items that implements this interface. Trumphed by <see cref="P:EPiServer.Shell.ViewConfiguration.Category" /> which is used to
	///       group the items (items without category will be considered a group too). Sort order will be respected within
	///       the group and the groups will be ordered by the lowest sort order within each group.
	///       </summary>
	public int SortOrder { get; set; }

	/// <summary>
	///       Used to group items that implements this interface.
	///       </summary>
	public string Category { get; set; }

	/// <summary>
	///       Gets or sets the plug-in areas where you can select the view.
	///       </summary>
	/// <value>
	///       The plug-in areas.
	///       </value>
	public IEnumerable<string> PlugInAreas { get; set; }

	public ViewFeatures AvailableFeatures { get; private set; } = new ViewFeatures();

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewConfiguration" /> class.
	///       </summary>
	/// <param name="forType">For type.</param>
	protected ViewConfiguration(Type forType)
	{
		_forType = forType;
	}
}

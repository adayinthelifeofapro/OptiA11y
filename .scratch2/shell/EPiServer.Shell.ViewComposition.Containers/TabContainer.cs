namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A tabbed layout container. The tabs can be positioned Top, Bottom, Leading and Center
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class TabContainer : ContainerBase
{
	/// <summary>
	///       Gets or sets the path used for calculating where to put the component in the UI
	///       for every tab on the tab container.
	///       </summary>
	public string ContainersPlugInArea
	{
		get
		{
			return (string)base.Settings["containersPlugInArea"];
		}
		set
		{
			base.Settings["containersPlugInArea"] = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.TabContainer" /> class.
	///       </summary>
	public TabContainer()
		: this(TabPosition.Top)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.TabContainer" /> class.
	///       </summary>
	/// <param name="tabPosition">The tab position.</param>
	public TabContainer(TabPosition tabPosition)
		: base("epi/shell/widget/TabContainer")
	{
		string value = null;
		switch (tabPosition)
		{
		case TabPosition.Left:
			value = "left-h";
			break;
		case TabPosition.Right:
			value = "right-h";
			break;
		case TabPosition.Top:
			value = "top";
			break;
		case TabPosition.Bottom:
			value = "bottom";
			break;
		}
		base.Settings["tabPosition"] = value;
		base.Settings["useMenu"] = true;
		base.Settings["useSlider"] = false;
	}
}

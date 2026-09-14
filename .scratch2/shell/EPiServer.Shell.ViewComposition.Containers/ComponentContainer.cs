namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       An extension of the grid container providing component management
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class ComponentContainer : ContainerBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ComponentContainer" /> class.
	///       </summary>
	public ComponentContainer()
		: this("epi/shell/widget/layout/ComponentContainer")
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ComponentContainer" /> class.
	///       </summary>
	protected ComponentContainer(string widgetType)
		: base(widgetType)
	{
		base.Settings.Add(new PersonalizableSetting("numberOfColumns", 1));
		base.Settings.Add(new Setting("showToolbar", personalizable: true));
		base.Settings.Add(new Setting("containerUnlocked", personalizable: false));
		base.Settings.Add(new Setting("closable", personalizable: false));
		base.Settings.Add(new Setting("personalizableHeading", string.Empty, personalizable: true));
	}
}

namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A tab container that is used within a <see cref="T:EPiServer.Shell.ViewComposition.Containers.ComponentContainer" /> to group components.
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class ComponentGroup : ContainerBase, ILocalizedHeadingComponent
{
	private const string Key = "headingLocalizationKey";

	/// <summary>
	///       Gets or sets the localization key for the heading.
	///       </summary>
	public string HeadingLocalizationKey
	{
		get
		{
			return base.Settings["headingLocalizationKey"] as string;
		}
		set
		{
			base.Settings["headingLocalizationKey"] = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ComponentGroup" /> class.
	///       </summary>
	public ComponentGroup()
		: base("epi/shell/widget/layout/ComponentTabContainer")
	{
		base.Settings.Add(new PersonalizableSetting("headingLocalizationKey", string.Empty));
	}
}

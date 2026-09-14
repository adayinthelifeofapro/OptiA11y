namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       An extension of the grid container providing component management.
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class ComponentPaneContainer : ComponentContainer
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ComponentPaneContainer" /> class.
	///       </summary>
	public ComponentPaneContainer()
		: base("epi/shell/widget/layout/ComponentPaneContainer")
	{
	}
}

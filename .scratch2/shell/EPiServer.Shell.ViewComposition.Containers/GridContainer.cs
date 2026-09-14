namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A container thas provides several layout areas where child components can be placed.
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class GridContainer : ContainerBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.GridContainer" /> class.
	///       </summary>
	public GridContainer()
		: base("dojox/layout/GridContainer")
	{
	}
}

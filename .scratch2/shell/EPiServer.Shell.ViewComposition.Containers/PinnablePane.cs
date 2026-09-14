namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A floating pane which can be pinned to be used inside the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderContainer" />.
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class PinnablePane : ContainerBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.PinnablePane" /> class.
	///       </summary>
	public PinnablePane()
		: base("epi/shell/layout/PinnablePane")
	{
	}
}

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Used to match components in a hierarchy. Both <see cref="M:EPiServer.Shell.ViewComposition.IComponentMatcher.MatchesComponent(EPiServer.Shell.ViewComposition.IComponent)" /> for the <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> and <see cref="M:EPiServer.Shell.ViewComposition.IContainerMatcher.MatchesContainer(EPiServer.Shell.ViewComposition.IContainer)" /> 
///       for the parent <see cref="T:EPiServer.Shell.ViewComposition.IContainer" /> must match in order to get a match.
///       </summary>
public interface IComponentMatcher : IContainerMatcher
{
	/// <summary>
	///       Answers if the given component is a match.
	///       </summary>
	/// <param name="component">The component to match.</param>
	/// <returns>
	///   <c>true</c> if the component is a match; otherwise <c>false</c>.</returns>
	bool MatchesComponent(IComponent component);
}

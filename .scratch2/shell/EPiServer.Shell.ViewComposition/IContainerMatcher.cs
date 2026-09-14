namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Used to match objects to a container. For instance when pluggin in an <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s to an <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />.
///       </summary>
public interface IContainerMatcher
{
	/// <summary>
	///       Answers if the given container is a match.
	///       </summary>
	/// <param name="container">The container to match.</param>
	/// <returns>
	///   <c>true</c> if the container is a match; otherwise <c>false</c>.</returns>
	bool MatchesContainer(IContainer container);
}

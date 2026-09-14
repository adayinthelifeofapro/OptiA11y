using System;

namespace EPiServer.Shell.ViewComposition;

internal class ComponentIdMatcher : IComponentMatcher, IContainerMatcher
{
	private readonly Guid _id;

	public ComponentIdMatcher(Guid id)
	{
		_id = id;
	}

	public bool MatchesComponent(IComponent component)
	{
		return _id == component.Id;
	}

	public bool MatchesContainer(IContainer container)
	{
		return true;
	}
}

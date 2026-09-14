using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using EPiServer.Framework;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Implements functionality for managing and creating components from component providers.
///       </summary>
public class ComponentManager : IComponentManager
{
	private List<IComponentDefinition> _componentDefinitions;

	private readonly List<IComponentProvider> _componentProviders;

	private readonly SecuredComponentOptions _options;

	private CompositeChangeToken _changeToken;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentManager" /> class.
	///       </summary>
	/// <param name="componentProviders">The component providers.</param>
	public ComponentManager(IEnumerable<IComponentProvider> componentProviders)
		: this(componentProviders, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ComponentManager" /> class.
	///       </summary>
	/// <param name="componentProviders">The component providers.</param>
	/// <param name="options">The configuration settings for components.</param>
	public ComponentManager(IEnumerable<IComponentProvider> componentProviders, SecuredComponentOptions options)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		base._002Ector();
		_componentProviders = componentProviders.ToList();
		_componentProviders.Sort((IComparer<IComponentProvider>?)new SortableComparer());
		_changeToken = new CompositeChangeToken(_componentProviders.Select((IComponentProvider p) => p.GetChangeToken()).ToArray());
		_changeToken.RegisterChangeCallback(ProviderChanged, null);
		_options = options;
	}

	/// <summary>
	///       Lists all registrered components.
	///       </summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing all components.</returns>
	public IEnumerable<IComponentDefinition> ListAll()
	{
		if (_componentDefinitions == null)
		{
			List<IComponentDefinition> list = new List<IComponentDefinition>();
			IList<SecuredComponentDetails> list2;
			if (_options != null)
			{
				list2 = _options.Components;
			}
			else
			{
				IList<SecuredComponentDetails> list3 = new List<SecuredComponentDetails>();
				list2 = list3;
			}
			IList<SecuredComponentDetails> source = list2;
			foreach (IComponentProvider componentProvider in _componentProviders)
			{
				IEnumerable<IComponentDefinition> componentDefinitions = componentProvider.GetComponentDefinitions();
				foreach (IComponentDefinition componentDefinition in componentDefinitions)
				{
					SecuredComponentDetails securedComponentDetails = source.FirstOrDefault((SecuredComponentDetails sc) => string.Equals(componentDefinition.DefinitionName, sc.DefinitionName, StringComparison.OrdinalIgnoreCase));
					if (securedComponentDetails == null)
					{
						continue;
					}
					foreach (string allowedRole in securedComponentDetails.AllowedRoles)
					{
						componentDefinition.AllowedRoles.Add(allowedRole);
					}
				}
				list.AddRange(componentDefinitions);
			}
			_componentDefinitions = list;
		}
		return _componentDefinitions;
	}

	/// <summary>
	///       Validates that the user has access to the component definition and creates a component from it.
	///       </summary>
	/// <param name="definition">The definition to create a component for.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>A component instance.</returns>
	public IComponent CreateComponent(IComponentDefinition definition, IPrincipal principal)
	{
		if (!definition.HasAccess(principal))
		{
			throw new UnauthorizedAccessException("User does not have access to component with name " + definition.DefinitionName);
		}
		foreach (IComponentProvider componentProvider in _componentProviders)
		{
			IComponent component = componentProvider.CreateComponent(definition);
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	/// <summary>
	///       Creates a component from a component definition name.
	///       </summary>
	/// <param name="componentDefinitionName">Name of the component definition.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> instance</returns>
	public IComponent CreateComponent(string componentDefinitionName, IPrincipal principal)
	{
		IComponentDefinition componentDefinition = GetComponentDefinition(componentDefinitionName);
		if (componentDefinition == null)
		{
			return null;
		}
		return CreateComponent(componentDefinition, principal);
	}

	/// <summary>
	///       Gets a component defintion by its name.
	///       </summary>
	/// <param name="name">The name of the definition to get.</param>
	/// <returns>
	///       The component definition having the supplied name, or null if none was found.
	///       </returns>
	public IComponentDefinition GetComponentDefinition(string name)
	{
		return ListAll().FirstOrDefault((IComponentDefinition d) => string.Equals(d.DefinitionName, name, StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	///       Lists all registrered components that matches the given criteria.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <param name="category">The category to match.</param>
	/// <param name="onlyShowComponentsAvailableForUserSelection">if set to <c>true</c> only shows components that has <see cref="P:EPiServer.Shell.ViewComposition.IPluggableComponentDefinition.IsAvailableForUserSelection" /> set to true.</param>
	/// <returns>
	///       An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing all components that matches the given criteria.
	///       </returns>
	public IEnumerable<IComponentDefinition> List(IPrincipal principal, string category, bool onlyShowComponentsAvailableForUserSelection)
	{
		return from d in ListAll()
			where (!onlyShowComponentsAvailableForUserSelection || d.IsAvailableForUserSelection) && (string.IsNullOrEmpty(category) || d.Categories.Contains(category)) && d.HasAccess(principal)
			select d;
	}

	/// <summary>
	///       Gets a list of all categories for the components.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>
	/// </returns>
	/// <value>The categories.</value>
	public IEnumerable<string> GetCategories(IPrincipal principal)
	{
		return new HashSet<string>((from d in ListAll()
			where d.Categories != null && d.Categories.Any() && d.HasAccess(principal)
			select d).SelectMany((IComponentDefinition d) => d.Categories));
	}

	private void ProviderChanged(object obj)
	{
		_componentDefinitions = null;
		_changeToken = new CompositeChangeToken(_componentProviders.Select((IComponentProvider p) => p.GetChangeToken()).ToArray());
		_changeToken.RegisterChangeCallback(ProviderChanged, null);
	}
}

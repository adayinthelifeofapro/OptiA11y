using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Provider that makes it possible to register a custom <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> for a <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" /> through the global <see cref="T:System.IServiceProvider" />.
///       </summary>
/// <remarks>Configuration is done by registering a type for the service type <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> with a <see cref="P:EPiServer.Shell.ViewComposition.IComponent.DefinitionName" /> that matches the <see cref="P:EPiServer.Shell.ViewComposition.IComponentDefinition.DefinitionName" />.</remarks>
[ComponentProvider]
public class ServiceLocatorComponentProvider : IComponentProvider, ISortable
{
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	///       Used to select the order of execution of the <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />s.
	///       </summary>
	/// <value>The sort order for this provider.</value>
	/// <remarks>This provider has a sort order of 50.</remarks>
	public int SortOrder => 50;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.ServiceLocatorComponentProvider" /> class.
	///       </summary>
	/// <param name="serviceProvider">The service provider.</param>
	public ServiceLocatorComponentProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	///       Gets the components that this provider provides.
	///       </summary>
	/// <returns>
	///       An <see cref="T:System.Collections.Generic.IEnumerable`1" /> with the component definitions that this provider handles.
	///       </returns>
	public IEnumerable<IComponentDefinition> GetComponentDefinitions()
	{
		return Array.Empty<IComponentDefinition>();
	}

	/// <summary>
	///       Try creating a component instance corresponding to an <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />.
	///       </summary>
	/// <param name="definition">The component definition to create a component for.</param>
	/// <returns>
	///   <c>true</c> if a component instance was created; otherwise <c>false</c>.
	///       </returns>
	public IComponent CreateComponent(IComponentDefinition definition)
	{
		return _serviceProvider.GetServices<IComponent>().LastOrDefault((IComponent c) => c.DefinitionName.Equals(definition.DefinitionName, StringComparison.OrdinalIgnoreCase));
	}
}

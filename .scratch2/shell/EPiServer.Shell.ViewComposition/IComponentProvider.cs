using System.Collections.Generic;
using EPiServer.Framework;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Definition of a provider for <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s. Provides a list of <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />s 
///       and the functionality to create <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />s from the definitions.
///       </summary>
public interface IComponentProvider : ISortable
{
	/// <summary>
	///       Gets the components that this provider provides.
	///       </summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> with the component definitions that this provider handles.</returns>
	IEnumerable<IComponentDefinition> GetComponentDefinitions();

	/// <summary>
	///       Creates component instance corresponding to an <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />.
	///       </summary>
	/// <param name="definition">The component definition to create a component for.</param>
	/// <returns>A component instance if one could be created; otherwise <c>null</c>.</returns>
	/// <remarks>This method should not perform any access control validation.</remarks>
	IComponent CreateComponent(IComponentDefinition definition);

	/// <summary>
	///       Defines a change token that can be used to signal changes.
	///       </summary>
	IChangeToken GetChangeToken()
	{
		return NullChangeToken.Singleton;
	}
}

using System.Collections.Generic;
using System.Security.Principal;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Interface to list components and component categories.
///       </summary>
public interface IComponentManager
{
	/// <summary>
	///       Gets a list of all categories for the components.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>
	/// </returns>
	/// <value>The categories.</value>
	IEnumerable<string> GetCategories(IPrincipal principal);

	/// <summary>
	///       Lists all registrered components that matches the given criteria.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <param name="category">The category to match.</param>
	/// <param name="onlyShowComponentsAvailableForUserSelection">if set to <c>true</c> only shows components that has <see cref="P:EPiServer.Shell.ViewComposition.IPluggableComponentDefinition.IsAvailableForUserSelection" /> set to true.</param>
	/// <returns>
	///       An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing all components that matches the given criteria.
	///       </returns>
	IEnumerable<IComponentDefinition> List(IPrincipal principal, string category, bool onlyShowComponentsAvailableForUserSelection);

	/// <summary>
	///       Lists all registrered components.
	///       </summary>
	/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> containing all components.</returns>
	/// <remarks>This method does not filter components for function access rights.</remarks>
	IEnumerable<IComponentDefinition> ListAll();

	/// <summary>
	///       Creates a component from the component definition.
	///       </summary>
	/// <param name="definition">The definition to create a component for.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>A component instance.</returns>
	/// <remarks>Will perform access control validation for the component definition and throw a <see cref="T:System.UnauthorizedAccessException" /> if the user does not have access to the component definition.</remarks>
	IComponent CreateComponent(IComponentDefinition definition, IPrincipal principal);

	/// <summary>
	///       Creates a component from a component definition name.
	///       </summary>
	/// <param name="componentDefinitionName">Name of the component definition.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.ViewComposition.IComponent" /> instance</returns>
	/// <remarks>If no component definition exists with the given name null will be returned.
	///       Will perform access control validation for the component definition and throw a <see cref="T:System.UnauthorizedAccessException" /> if the user does not have access to the component definition.</remarks>
	IComponent CreateComponent(string componentDefinitionName, IPrincipal principal);

	/// <summary>
	///       Gets a component defintion by its name.
	///       </summary>
	/// <param name="name">The name of the definition to get.</param>
	/// <returns>The component definition having the supplied name, or null if none was found.</returns>
	IComponentDefinition GetComponentDefinition(string name);
}

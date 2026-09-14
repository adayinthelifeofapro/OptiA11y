using System.Collections.Generic;
using System.Security.Principal;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a component that can be added or removed automatically in a view.
///       </summary>
public interface IPluggableComponentDefinition : IContainerMatcher
{
	/// <summary>
	///       Gets a value indicating whether this definition can be plugged in automatically to an <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />.
	///       </summary>
	/// <value>
	///   <c>true</c> if this definition can be plugged in automatically; otherwise, <c>false</c>.
	///       </value>
	bool SupportsAutomaticRegistration { get; }

	/// <summary>
	///       Gets a value indicating whether this component should be selectable by a user when customizing a panel.
	///       </summary>
	/// <value>If this component should be selectable by a user when customizing a panel.</value>
	bool IsAvailableForUserSelection { get; }

	/// <summary>
	///       Gets the list of roles that are allowed to use this component.
	///       </summary>
	/// <value>The role list .</value>
	ICollection<string> AllowedRoles { get; }

	/// <summary>
	///       Creates the component corresponding to this component definition.
	///       </summary>
	/// <returns>A new instance of an <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.</returns>
	/// <remarks>This method should not perform any access control validation.</remarks>
	IComponent CreateComponent();

	/// <summary>
	///       Determines whether the specified principal has access to this component.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>
	///   <c>true</c> if the specified principal has access; otherwise, <c>false</c>.
	///       </returns>
	bool HasAccess(IPrincipal principal);
}

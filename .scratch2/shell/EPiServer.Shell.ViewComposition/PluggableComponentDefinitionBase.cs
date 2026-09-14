using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Base implementation of a component that can be added or removed automatically in a view.
///       </summary>
public abstract class PluggableComponentDefinitionBase : IPluggableComponentDefinition, IContainerMatcher
{
	/// <summary>
	///       Gets or sets the plug in areas that the component should automatically plug into.
	///       </summary>
	/// <value>The plug in areas.</value>
	public IEnumerable<string> PlugInAreas { get; protected set; }

	/// <summary>
	///       Gets or sets if this component should be selectable by a user when customizing a panel.
	///       </summary>
	/// <value>
	///       If this component should be selectable by a user when customizing a panel.
	///       </value>
	public bool IsAvailableForUserSelection { get; protected set; }

	/// <summary>
	///       Gets a value indicating whether this definition can be plugged in automatically to an <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />.
	///       </summary>
	/// <value>
	///   <c>true</c> if this definition can be plugged in automatically; otherwise, <c>false</c>.
	///       </value>
	/// <remarks>This should only be set to true for components that use the auto plug-in-feature for performance reasons.</remarks>
	public virtual bool SupportsAutomaticRegistration
	{
		get
		{
			if (PlugInAreas != null)
			{
				return PlugInAreas.Any();
			}
			return false;
		}
	}

	/// <summary>
	///       Gets or sets the list of roles that are allowed to use this component.
	///       </summary>
	/// <value>The role list .</value>
	public ICollection<string> AllowedRoles { get; private set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PluggableComponentDefinitionBase" /> class.
	///       </summary>
	protected PluggableComponentDefinitionBase()
	{
		AllowedRoles = new HashSet<string>();
		PlugInAreas = Array.Empty<string>();
	}

	/// <summary>
	///       Defines if the component should be added automatically to an <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />.
	///       </summary>
	/// <param name="container">The container to match.</param>
	/// <returns>
	///   <c>true</c> if the component should be added to the <see cref="T:EPiServer.Shell.ViewComposition.IContainer" />; otherwise <c>false</c>.
	///       </returns>
	public bool MatchesContainer(IContainer container)
	{
		if (PlugInAreas != null)
		{
			return PlugInAreas.Any((string path) => string.Equals(path, container.PlugInArea, StringComparison.OrdinalIgnoreCase));
		}
		return false;
	}

	/// <summary>
	///       Determines whether the specified principal has access to this component.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>
	///   <c>true</c> if the specified principal has access; otherwise, <c>false</c>.
	///       </returns>
	public bool HasAccess(IPrincipal principal)
	{
		if (AllowedRoles == null || AllowedRoles.Count == 0)
		{
			return true;
		}
		if (AllowedRoles.Count == 1 && string.Equals(AllowedRoles.First(), "none", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return AllowedRoles.Any(principal.IsInRole);
	}

	/// <summary>
	///       Creates the component corresponding to this component definition.
	///       </summary>
	/// <returns>
	///       A new instance of an <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.
	///       </returns>
	/// <remarks>This method does not perform any access control validation.</remarks>
	public abstract IComponent CreateComponent();
}

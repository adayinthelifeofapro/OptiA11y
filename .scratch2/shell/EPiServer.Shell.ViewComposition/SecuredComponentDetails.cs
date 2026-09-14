using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       A components security settings.
///       </summary>
public class SecuredComponentDetails
{
	/// <summary>
	///       The definition name of the component to match.
	///       </summary>
	/// <remarks>This should match either the <see cref="P:EPiServer.Shell.ViewComposition.IComponent.DefinitionName" /> or <see cref="P:EPiServer.Shell.ViewComposition.IComponentDefinition.DefinitionName" /> depending on the type of transformation.</remarks>
	public string DefinitionName { get; set; }

	/// <summary>
	///       The allowed roles for this component type.
	///       </summary>
	public IList<string> AllowedRoles { get; } = new List<string>();
}

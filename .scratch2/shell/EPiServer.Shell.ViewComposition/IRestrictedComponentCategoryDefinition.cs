using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Used to define a restricted range of component types, for instance for a view.
///       </summary>
public interface IRestrictedComponentCategoryDefinition
{
	/// <summary>
	///       The component categories.
	///       </summary>
	IEnumerable<string> GetComponentCategories();
}

using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Provides property mappings between the name in the property collection, and the name on the interface.
///       </summary>
/// <remarks>Used internally by the UI. Should not need to be implemented by third party users.</remarks>
public interface IMetadataPropertyMappingProvider
{
	/// <summary>
	///       Gets the property mappings between the name in the property collection, and the name on the interface.
	///       </summary>
	/// <param name="container">The container that has the properties to get mappings for.</param>
	/// <returns>A dictionary containing mappings</returns>
	IEnumerable<PropertyMapping> GetPropertyMappings(object container);
}

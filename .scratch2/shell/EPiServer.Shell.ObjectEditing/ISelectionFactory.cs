using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Creates a list of selection items for a specific property.
///       </summary>
public interface ISelectionFactory
{
	/// <summary>
	///       Creates a list of selection items for a specific property.
	///       </summary>
	/// <param name="metadata">The metadata for a property.</param>
	/// <returns>A list of selection items for a specific property.</returns>
	IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata);
}

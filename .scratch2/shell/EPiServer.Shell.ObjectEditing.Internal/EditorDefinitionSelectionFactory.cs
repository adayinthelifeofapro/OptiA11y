using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.ObjectEditing.Internal;

internal class EditorDefinitionSelectionFactory : ISelectionFactory
{
	public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
	{
		if (metadata.EditorConfiguration.TryGetValue("selections", out var value) && value is IEnumerable<ISelectItem> result)
		{
			return result;
		}
		return Enumerable.Empty<ISelectItem>();
	}
}

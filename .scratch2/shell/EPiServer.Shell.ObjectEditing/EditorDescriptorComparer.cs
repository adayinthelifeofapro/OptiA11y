using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace EPiServer.Shell.ObjectEditing;

internal class EditorDescriptorComparer : IEqualityComparer<EditorDescriptor>
{
	public bool Equals(EditorDescriptor x, EditorDescriptor y)
	{
		return x.GetType() == y.GetType();
	}

	public int GetHashCode(EditorDescriptor obj)
	{
		return obj.GetType().GetHashCode();
	}
}

using System;

namespace EPiServer.Shell.ObjectEditing.Internal;

internal static class TypeSerializer
{
	/// <summary>
	///       Gets the name of the type including the name of its assembly.
	///       It's different from <see cref="P:System.Type.AssemblyQualifiedName" /> in that it doesn't include version.
	///       </summary>
	/// <param name="dataType">
	/// </param>
	/// <returns>
	/// </returns>
	public static string GetAssemblyTypeNameWithoutVersion(Type dataType)
	{
		return dataType.FullName + "," + dataType.Assembly.GetName().Name;
	}
}

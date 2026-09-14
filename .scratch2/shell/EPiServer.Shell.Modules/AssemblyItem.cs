using System.Collections.Generic;
using System.Reflection;

namespace EPiServer.Shell.Modules;

public class AssemblyItem
{
	public Assembly Assembly { get; }

	public IList<AssemblyItem> Dependencies { get; } = new List<AssemblyItem>();

	public AssemblyItem(Assembly assembly)
	{
		Assembly = assembly;
	}
}

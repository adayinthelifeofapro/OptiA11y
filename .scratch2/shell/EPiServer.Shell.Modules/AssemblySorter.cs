using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EPiServer.Shell.Modules;

public class AssemblySorter
{
	public IEnumerable<Assembly> GetSortedAssemblies(IEnumerable<Assembly> assemblies)
	{
		List<AssemblyItem> list = assemblies.Select((Assembly a) => new AssemblyItem(a)).ToList();
		foreach (AssemblyItem item in list)
		{
			AssemblyName[] referencedAssemblies = item.Assembly.GetReferencedAssemblies();
			foreach (AssemblyName reference in referencedAssemblies)
			{
				AssemblyItem assemblyItem = list.FirstOrDefault((AssemblyItem i) => i.Assembly.FullName == reference.FullName);
				if (assemblyItem != null)
				{
					item.Dependencies.Add(assemblyItem);
				}
			}
		}
		return from x in list.TopologicalSort((AssemblyItem x) => x.Dependencies, new AssemblyItemEqualityComparer())
			select x.Assembly;
	}
}

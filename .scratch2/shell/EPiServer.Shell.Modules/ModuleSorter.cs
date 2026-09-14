using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Modules;

public class ModuleSorter
{
	public IEnumerable<ShellModule> GetSortedShellModules(IEnumerable<ShellModule> modules)
	{
		List<ShellModuleItem> list = modules.Select((ShellModule a) => new ShellModuleItem(a)).ToList();
		foreach (ShellModuleItem item in list)
		{
			if (item.ShellModule?.Manifest?.ClientModule == null)
			{
				continue;
			}
			foreach (ModuleDependency reference in item.ShellModule.Manifest.ClientModule.ModuleDependencies)
			{
				ShellModuleItem shellModuleItem = list.FirstOrDefault((ShellModuleItem i) => i.ShellModule.Name == reference.Dependency);
				if (shellModuleItem != null)
				{
					item.Dependencies.Add(shellModuleItem);
				}
			}
		}
		return from x in list.TopologicalSort((ShellModuleItem x) => x.Dependencies, new ShellModuleItemEqualityComparer())
			select x.ShellModule;
	}
}

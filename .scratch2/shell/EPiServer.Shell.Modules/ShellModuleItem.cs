using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

public class ShellModuleItem
{
	public ShellModule ShellModule { get; }

	public IList<ShellModuleItem> Dependencies { get; } = new List<ShellModuleItem>();

	public ShellModuleItem(ShellModule shellModule)
	{
		ShellModule = shellModule;
	}
}

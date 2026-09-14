using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

public class ShellModuleItemEqualityComparer : EqualityComparer<ShellModuleItem>
{
	public override bool Equals(ShellModuleItem x, ShellModuleItem y)
	{
		if (x != null || y != null)
		{
			if (x != null && y != null)
			{
				return x.ShellModule.Name == y.ShellModule.Name;
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode(ShellModuleItem obj)
	{
		return obj?.ShellModule.Name.GetHashCode() ?? 0;
	}
}

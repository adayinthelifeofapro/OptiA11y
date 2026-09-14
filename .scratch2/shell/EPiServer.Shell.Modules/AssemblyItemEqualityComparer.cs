using System.Collections.Generic;

namespace EPiServer.Shell.Modules;

public class AssemblyItemEqualityComparer : EqualityComparer<AssemblyItem>
{
	public override bool Equals(AssemblyItem x, AssemblyItem y)
	{
		if (x != null || y != null)
		{
			if (x != null && y != null)
			{
				return x.Assembly.FullName == y.Assembly.FullName;
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode(AssemblyItem obj)
	{
		return obj?.Assembly.FullName.GetHashCode() ?? 0;
	}
}

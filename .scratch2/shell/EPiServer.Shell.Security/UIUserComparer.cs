using System.Collections.Generic;

namespace EPiServer.Shell.Security;

internal class UIUserComparer : IEqualityComparer<IUIUser>
{
	public virtual bool Equals(IUIUser x, IUIUser y)
	{
		if (x == y)
		{
			return true;
		}
		if (x == null || y == null)
		{
			return false;
		}
		if (x.Username == y.Username && x.Email == y.Email)
		{
			return x.ProviderName == y.ProviderName;
		}
		return false;
	}

	public virtual int GetHashCode(IUIUser user)
	{
		HashCodeCombiner hashCodeCombiner = HashCodeCombiner.Start();
		if (user != null)
		{
			hashCodeCombiner.Add(user.Username);
			hashCodeCombiner.Add(user.Email);
			hashCodeCombiner.Add(user.ProviderName);
		}
		return hashCodeCombiner;
	}
}

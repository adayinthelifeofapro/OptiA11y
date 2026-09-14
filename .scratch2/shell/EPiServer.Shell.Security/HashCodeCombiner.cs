using System.Collections;
using System.Collections.Generic;

namespace EPiServer.Shell.Security;

internal struct HashCodeCombiner
{
	private long _combinedHash64;

	public readonly int CombinedHash => _combinedHash64.GetHashCode();

	private HashCodeCombiner(long seed)
	{
		_combinedHash64 = seed;
	}

	public void Add(IEnumerable e)
	{
		if (e == null)
		{
			Add(0);
			return;
		}
		int num = 0;
		foreach (object item in e)
		{
			Add(item);
			num++;
		}
		Add(num);
	}

	public static implicit operator int(HashCodeCombiner self)
	{
		return self.CombinedHash;
	}

	public void Add(int i)
	{
		_combinedHash64 = ((_combinedHash64 << 5) + _combinedHash64) ^ i;
	}

	public void Add(string s)
	{
		int i = s?.GetHashCode() ?? 0;
		Add(i);
	}

	public void Add(object o)
	{
		int i = o?.GetHashCode() ?? 0;
		Add(i);
	}

	public void Add<TValue>(TValue value, IEqualityComparer<TValue> comparer)
	{
		int i = ((value != null) ? comparer.GetHashCode(value) : 0);
		Add(i);
	}

	public static HashCodeCombiner Start()
	{
		return new HashCodeCombiner(5381L);
	}
}

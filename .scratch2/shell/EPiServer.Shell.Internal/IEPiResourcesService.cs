using System.Collections.Generic;
using System.Globalization;

namespace EPiServer.Shell.Internal;

public interface IEPiResourcesService
{
	Dictionary<string, object> GetResources(string resourceKey, string userName);

	Dictionary<string, object> GetResources(string resourceKey, CultureInfo culture);

	string GetResourceKey(string typeName);

	void Merge<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> target);

	bool IsSameCulture(CultureInfo left, CultureInfo right);
}

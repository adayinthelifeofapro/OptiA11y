using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using EPiServer.Framework.Localization;
using EPiServer.Shell.Profile.Internal;

namespace EPiServer.Shell.Internal;

internal class EPiResourcesService : IEPiResourcesService
{
	private readonly LocalizationService _localizationService;

	private readonly ICurrentUiCulture _currentUiCulture;

	public EPiResourcesService(LocalizationService localizationService, ICurrentUiCulture currentUiCulture)
	{
		_localizationService = localizationService;
		_currentUiCulture = currentUiCulture;
	}

	public string GetResourceKey(string typeName)
	{
		return ClientResourceKeyConverter.TransformResourceKey(HttpUtility.JavaScriptStringEncode(typeName));
	}

	public Dictionary<string, object> GetResources(string resourceKey, CultureInfo culture)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		char[] separator = new char[1] { '/' };
		IEnumerable<ResourceItem> enumerable = _localizationService.GetAllStringsByCulture(resourceKey, culture);
		CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en");
		if (!IsSameCulture(cultureInfo, culture) && !IsSameCulture(cultureInfo, _localizationService.FallbackCulture))
		{
			Dictionary<string, ResourceItem> dictionary = enumerable.ToDictionary<ResourceItem, string>((ResourceItem item) => ((ResourceItem)(ref item)).Key, StringComparer.OrdinalIgnoreCase);
			AddFallbackStringsToDictionary(dictionary, resourceKey, cultureInfo);
			enumerable = dictionary.Values;
		}
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		foreach (ResourceItem item in enumerable)
		{
			ResourceItem current = item;
			IDictionary<string, object> dictionary3 = dictionary2;
			string[] array = ((ResourceItem)(ref current)).Key.Substring(resourceKey.Length).Split(separator, StringSplitOptions.RemoveEmptyEntries);
			for (int num = 0; num < array.Length; num++)
			{
				string key = array[num].ToLowerInvariant();
				if (dictionary3.TryGetValue(key, out var value))
				{
					object obj = value;
					if (obj is string)
					{
						break;
					}
					if (obj is Dictionary<string, object>)
					{
						dictionary3 = obj as Dictionary<string, object>;
					}
				}
				else if (num + 1 == array.Length)
				{
					dictionary3[key] = ((ResourceItem)(ref current)).Value;
				}
				else
				{
					Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
					dictionary3.Add(key, dictionary4);
					dictionary3 = dictionary4;
				}
			}
		}
		return dictionary2;
	}

	public Dictionary<string, object> GetResources(string resourceKey, string userName)
	{
		CultureInfo culture = _currentUiCulture.Get(userName);
		return GetResources(resourceKey, culture);
	}

	public bool IsSameCulture(CultureInfo left, CultureInfo right)
	{
		if (left == null || right == null)
		{
			return false;
		}
		if (!left.Equals(right) && !left.Parent.Equals(right))
		{
			return right.Parent.Equals(left);
		}
		return true;
	}

	public void Merge<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> target)
	{
		foreach (KeyValuePair<TKey, TValue> item in target)
		{
			source[item.Key] = item.Value;
		}
	}

	protected virtual void AddFallbackStringsToDictionary(Dictionary<string, ResourceItem> targetResources, string resourceKey, CultureInfo defaultCulture)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		List<string> targetKeys = targetResources.Keys.Select((string k) => k.ToLower()).ToList();
		foreach (ResourceItem item in from r in _localizationService.GetAllStringsByCulture(resourceKey, defaultCulture)
			where !targetKeys.Contains(((ResourceItem)(ref r)).Key.ToLower())
			select r)
		{
			ResourceItem current = item;
			targetResources.Add(((ResourceItem)(ref current)).Key, current);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Text;

namespace EPiServer.Shell.Resources;

/// <summary>
///       Provides convenient access to culture-specific client resources at run time
///       Use GetResourcesAsJson to get the JSON object.
///       </summary>
public class JsonResourceSerializer : ResourceManager
{
	private readonly CultureInfo _neutralCulture;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Resources.JsonResourceSerializer" /> class.
	///       </summary>
	/// <param name="resourceType">A System.Type from which the System.Resources.ResourceManager derives all information for finding .resources files.</param>
	public JsonResourceSerializer(Type resourceType)
		: base(resourceType)
	{
		_neutralCulture = ResourceManager.GetNeutralResourcesLanguage(resourceType.Assembly);
	}

	/// <summary>
	///       Gets the resources as a JSON object. The Resources for the cultre are merged with the
	///       neutral cultures for the type which resource manager were created for.
	///       This means that if you are asking for a resource in Swedish and it does not exist you will get the resource
	///       in the language which the assmbly has set as neutral.
	///       </summary>
	/// <param name="culture">The culture you want your resources in.</param>
	/// <returns>A string representing the JSON object. "{[ResourceKey]:[ResourceValue], [ResourceKey]:[ResourceValue]}" </returns>
	/// <remarks>
	///       Will only return resources that are of type <see cref="T:System.String" /></remarks>
	public virtual string GetResourcesAsJson(CultureInfo culture)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> mergedResource in GetMergedResources(culture))
		{
			stringBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0}:\"{1}\",", mergedResource.Key, mergedResource.Value));
		}
		return string.Format(CultureInfo.CurrentCulture, "{{{0}}}", stringBuilder.ToString().Trim(','));
	}

	/// <summary>
	///       Merges the resources for cultureInfo with the neutral resources
	///       </summary>
	/// <param name="culture">The culture you want your resources in</param>
	/// <returns>Dictionary containing the merged resources</returns>
	/// <remarks>
	///       Will only return resources that are of type <see cref="T:System.String" /></remarks>
	protected virtual Dictionary<string, string> GetMergedResources(CultureInfo culture)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (DictionaryEntry item in GetResourceSet(_neutralCulture, createIfNotExists: true, tryParents: true))
		{
			if (item.Value is string)
			{
				dictionary[item.Key.ToString()] = item.Value.ToString().Replace("\"", "\\\"");
			}
		}
		foreach (DictionaryEntry item2 in GetResourceSet(culture, createIfNotExists: true, tryParents: true))
		{
			if (item2.Value is string)
			{
				dictionary[item2.Key.ToString()] = item2.Value.ToString().Replace("\"", "\\\"");
			}
		}
		return dictionary;
	}
}

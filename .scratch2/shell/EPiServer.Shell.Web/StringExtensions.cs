using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Web;

/// <summary>
///       Extension methods for the String class.
///       </summary>
public static class StringExtensions
{
	private const string ControllerSuffix = "Controller";

	/// <summary>
	///       Removes one instance of a text from the end of a string.
	///       </summary>
	public static string TrimEnd(this string text, string textToTrim)
	{
		if (text == null || textToTrim == null)
		{
			return text;
		}
		if (text.EndsWith(textToTrim, StringComparison.CurrentCulture))
		{
			int length = textToTrim.Length;
			return text.Substring(0, text.Length - length);
		}
		return text;
	}

	/// <summary>
	///       Trims excessivly long strings replacing it's end with ...
	///       </summary>
	public static string Ellipsis(this string text, int maxLength)
	{
		if (maxLength < 3)
		{
			throw new ArgumentException("'maxLength' needs to be at least 3", "maxLength");
		}
		if (text == null || text.Length <= maxLength)
		{
			return text;
		}
		return text.Substring(0, maxLength - 3) + "...";
	}

	/// <summary>
	///       Flattens a dictionary into a string that can be used for debugging purposes
	///       </summary>
	/// <typeparam name="TK">Ignored</typeparam>
	/// <typeparam name="TV">Ignored</typeparam>
	/// <param name="map">The dictionary to flatten out</param>
	/// <param name="separator">The separator between key-value paris in the dictionary</param>
	/// <returns>A string representing the dictionary values</returns>
	internal static string ToKeyValueString<TK, TV>(this IDictionary<TK, TV> map, string separator)
	{
		if (map == null)
		{
			return null;
		}
		return string.Join(separator, map.Select((KeyValuePair<TK, TV> kvp) => kvp.Key?.ToString() + "=" + kvp.Value).ToArray());
	}

	/// <summary>
	///       Removes the "Controller" suffix from the given controller name.
	///       </summary>
	/// <param name="controllerName">A string representing a controller.</param>
	/// <returns>A trimmed name without the controller ending.</returns>
	public static string TrimControllerSuffix(string controllerName)
	{
		return TrimEnd(controllerName, "Controller");
	}

	/// <summary>Returns a value indicating whether a specified substring occurs within this string.</summary>
	/// <param name="source">
	/// </param>
	/// <param name="value">The string to seek</param>
	/// <param name="comparison">One of the enumeration values that specifies the rules for the search</param>
	/// <returns>
	///   <see langword="true" /> if the <paramref name="value" /> parameter occurs within this string, or if <paramref name="value" /> is the empty string (""); otherwise, <see langword="false" />.</returns>
	public static bool Contains(this string source, string value, StringComparison comparison)
	{
		if (source == null)
		{
			return false;
		}
		return source.IndexOf(value, comparison) >= 0;
	}

	/// <summary>
	///       Produce lowerCamelCase strings
	///       </summary>
	public static string NameCamelCase(this string value)
	{
		if (string.IsNullOrWhiteSpace(value) || value.Length < 1)
		{
			return value;
		}
		return char.ToLowerInvariant(value[0]) + value.Substring(1);
	}
}

using System;

namespace EPiServer.Shell.Internal;

/// <summary>
///       Utility class for converting resources keys.
///       </summary>
public static class ClientResourceKeyConverter
{
	/// <summary>
	///       Transforms a resource key that has the character "." as separator to a key with slashes as separator.
	///       </summary>
	/// <param name="resourceKeyWithDotSeparator">A resource key with dot as separator.</param>
	/// <returns>A resource key with slash as separator.</returns>
	/// <example>From A.Resource.Key to /A/Resource/Key</example>
	public static string TransformResourceKey(string resourceKeyWithDotSeparator)
	{
		ArgumentException.ThrowIfNullOrEmpty(resourceKeyWithDotSeparator, "resourceKeyWithDotSeparator");
		string text = resourceKeyWithDotSeparator.Replace('.', '/');
		if (!text.StartsWith("/", StringComparison.CurrentCultureIgnoreCase))
		{
			text = "/" + text;
		}
		return text;
	}
}

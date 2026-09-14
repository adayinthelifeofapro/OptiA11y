using System.Globalization;
using System.Text;

namespace EPiServer.Shell.Serialization.Json.Internal;

/// <summary>
///       String utilities for Json contract resolvers and converters.
///       </summary>
public static class StringUtility
{
	/// <summary>
	///       Convert a string to camel case.
	///       </summary>
	/// <param name="input">The input string.</param>
	public static string ToCamelCase(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		if (!char.IsUpper(input[0]))
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < input.Length; i++)
		{
			bool flag = i + 1 < input.Length;
			if (i == 0 || !flag || !char.IsLetter(input[i + 1]) || char.IsUpper(input[i + 1]))
			{
				char value = char.ToLower(input[i], CultureInfo.InvariantCulture);
				stringBuilder.Append(value);
				continue;
			}
			stringBuilder.Append(input.Substring(i));
			break;
		}
		return stringBuilder.ToString();
	}
}

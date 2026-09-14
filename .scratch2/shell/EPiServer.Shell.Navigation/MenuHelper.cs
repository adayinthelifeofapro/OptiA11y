using System.Globalization;
using System.IO;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Methods used to render menus in various locations.
///       </summary>
public static class MenuHelper
{
	/// <summary>
	///       Ensures the the path is valid. Removes trailing / and appends missing / at the start
	///       </summary>
	internal static string EnsureValidMenuPath(this string path)
	{
		if (path.EndsWith('/'))
		{
			string text = path;
			path = text.Substring(0, text.Length - 1);
		}
		if (!path.StartsWith('/'))
		{
			path = "/" + path;
		}
		return path;
	}

	/// <summary>
	///       Calculates depth of the menu path.
	///       </summary>
	/// <param name="path">The path to calculate on</param>
	/// <returns>The depth of the patch</returns>
	internal static int CalculatePathDepth(this string path)
	{
		int num = 0;
		for (int i = 0; i < path.Length; i++)
		{
			if (path[i] == '/')
			{
				num++;
			}
		}
		return num;
	}

	internal static void WriteFormat(this TextWriter writer, string format, params object[] args)
	{
		writer.Write(string.Format(CultureInfo.InvariantCulture, format, args));
	}

	internal static void WriteFormatUnlessEmpty(this TextWriter writer, string format, string argument)
	{
		if (!string.IsNullOrEmpty(argument))
		{
			writer.Write(string.Format(CultureInfo.InvariantCulture, format, argument));
		}
	}

	internal static string GetClientId(this MenuItem menuItem)
	{
		return menuItem.Path.Trim('/').Replace('/', '_');
	}

	internal static string GetSubmenuClientId(this MenuItem menuItem)
	{
		if (menuItem == null)
		{
			return null;
		}
		return menuItem.Path.Trim('/').Replace('/', '_') + "_sub";
	}
}

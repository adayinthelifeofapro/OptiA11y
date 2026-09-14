using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Adapted Version utility, provides methods to get <see cref="T:System.Runtime.Versioning.FrameworkName" /> by folder name and find compatible assemblies.
///       </summary>
public static class VersionUtility
{
	/// <summary>
	///       .NET framework indentifier
	///       </summary>
	public const string NetFrameworkIdentifier = ".NETFramework";

	/// <summary>
	///       Silverlight indentifier
	///       </summary>
	public const string SilverlightIdentifier = "Silverlight";

	/// <summary>
	///       Identifier for all unsupported frameworks
	///       </summary>
	public const string UnsupportedFrameworkIdentifier = "Unsupported";

	/// <summary>
	///       The unsupported framework value
	///       </summary>
	public static readonly FrameworkName UnsupportedFrameworkName = new FrameworkName("Unsupported", new Version());

	private static readonly Version _emptyVersion = new Version();

	private static readonly Dictionary<string, string> _knownIdentifiers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "NET", ".NETFramework" },
		{ ".NET", ".NETFramework" },
		{ "NETFramework", ".NETFramework" },
		{ ".NETFramework", ".NETFramework" },
		{ "SL", "Silverlight" },
		{ "Silverlight", "Silverlight" }
	};

	private static readonly Dictionary<string, string> _knownProfiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "Client", "Client" },
		{
			"Full",
			string.Empty
		}
	};

	private static readonly Dictionary<string, Dictionary<string, string[]>> _compatibiltyMapping = new Dictionary<string, Dictionary<string, string[]>>(StringComparer.OrdinalIgnoreCase) { 
	{
		".NETFramework",
		new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"",
				new string[1] { "Client" }
			},
			{
				"Client",
				new string[1] { "" }
			}
		}
	} };

	/// <summary>
	///       Gets the default target framework version.
	///       </summary>
	public static Version DefaultTargetFrameworkVersion
	{
		get
		{
			Version version = typeof(string).Assembly.GetName().Version;
			if (version != null && version.Major == 4 && version.Minor == 0)
			{
				return new Version(4, 5, 0, 0);
			}
			return version;
		}
	}

	/// <summary>
	///       Gets the default target framework.
	///       </summary>
	/// <value>
	///       The default target framework.
	///       </value>
	public static FrameworkName DefaultTargetFramework => new FrameworkName(".NETFramework", DefaultTargetFrameworkVersion);

	/// <summary>
	///       This function tries to normalize a string that represents framework version names into
	///       something a framework name that the package manager understands.
	///       </summary>
	public static FrameworkName ParseFrameworkName(string frameworkName)
	{
		ArgumentNullException.ThrowIfNull(frameworkName, "frameworkName");
		string text = null;
		string text2 = null;
		string[] array = frameworkName.Split('-');
		if (array.Length > 2)
		{
			throw new ArgumentException("Invalid framework name format.", "frameworkName");
		}
		string text3 = ((array.Length != 0) ? array[0].Trim() : null);
		string text4 = ((array.Length > 1) ? array[1].Trim() : null);
		if (string.IsNullOrEmpty(text3))
		{
			throw new ArgumentException("Framework name is null or empty.", "frameworkName");
		}
		Match match = Regex.Match(text3, "\\d+");
		if (match.Success)
		{
			text = text3.Substring(0, match.Index).Trim();
			text2 = text3.Substring(match.Index).Trim();
		}
		else
		{
			text = text3.Trim();
		}
		if (!string.IsNullOrEmpty(text) && !_knownIdentifiers.TryGetValue(text, out text))
		{
			return UnsupportedFrameworkName;
		}
		if (!string.IsNullOrEmpty(text4) && _knownProfiles.TryGetValue(text4, out var value))
		{
			text4 = value;
		}
		if (int.TryParse(text2, out var _))
		{
			if (text2.Length > 4)
			{
				text2 = text2.Substring(0, 4);
			}
			text2 = text2.PadRight(2, '0');
			text2 = string.Join(".", text2.Select((char ch) => ch.ToString()));
		}
		if (!Version.TryParse(text2, out Version result2))
		{
			if (string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				return UnsupportedFrameworkName;
			}
			result2 = _emptyVersion;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = ".NETFramework";
		}
		return new FrameworkName(text, result2, text4);
	}

	/// <summary>
	///       Tries the get compatible module assemblies.
	///       </summary>
	/// <typeparam name="T">
	/// </typeparam>
	/// <param name="projectFramework">The project framework.</param>
	/// <param name="items">The module assemblies.</param>
	/// <param name="compatibleItems">The compatible modules assemblies.</param>
	/// <returns>
	/// </returns>
	public static bool TryGetCompatibleItems<T>(FrameworkName projectFramework, IEnumerable<T> items, out IEnumerable<T> compatibleItems) where T : ModuleAssembly
	{
		if (!items.Any())
		{
			compatibleItems = Enumerable.Empty<T>();
			return true;
		}
		FrameworkName defaultFramework = new FrameworkName(projectFramework.Identifier, new Version(), projectFramework.Profile);
		IEnumerable<IGrouping<FrameworkName, T>> source = from _003C_003Eh__TransparentIdentifier0 in items.Select(delegate(T item)
			{
				IEnumerable<FrameworkName> frameworks;
				if (!item.SupportedFrameworks.Any())
				{
					IEnumerable<FrameworkName> enumerable = new FrameworkName[1];
					frameworks = enumerable;
				}
				else
				{
					frameworks = item.SupportedFrameworks;
				}
				return new { item, frameworks };
			})
			from framework in _003C_003Eh__TransparentIdentifier0.frameworks
			select new
			{
				Item = _003C_003Eh__TransparentIdentifier0.item,
				TargetFramework = framework
			} into g
			group g.Item by g.TargetFramework ?? defaultFramework;
		compatibleItems = (from g in source
			where IsCompatible(projectFramework, g.Key)
			orderby GetProfileCompatibility(projectFramework, g.Key) descending, g.Key.Version descending
			select g).FirstOrDefault();
		if (compatibleItems != null)
		{
			return compatibleItems.Any();
		}
		return false;
	}

	/// <summary>
	///       Normalizes the version.
	///       </summary>
	/// <param name="version">The verison.</param>
	/// <returns>
	/// </returns>
	public static Version NormalizeVersion(Version version)
	{
		return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
	}

	/// <summary>
	///       Determines whether the specified framework name is compatible with target framework name.
	///       </summary>
	/// <param name="frameworkName">Name of the framework.</param>
	/// <param name="targetFrameworkName">Name of the target framework.</param>
	/// <returns>
	///   <c>true</c> if the specified framework name is compatible; otherwise, <c>false</c>.
	///       </returns>
	public static bool IsCompatible(FrameworkName frameworkName, FrameworkName targetFrameworkName)
	{
		if (!frameworkName.Identifier.Equals(targetFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		if (NormalizeVersion(frameworkName.Version) < NormalizeVersion(targetFrameworkName.Version))
		{
			return false;
		}
		if (string.Equals(frameworkName.Profile, targetFrameworkName.Profile, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (_compatibiltyMapping.TryGetValue(frameworkName.Identifier, out var value) && value.TryGetValue(targetFrameworkName.Profile, out var value2))
		{
			return value2.Contains<string>(frameworkName.Profile, StringComparer.OrdinalIgnoreCase);
		}
		return false;
	}

	/// <summary>
	///       Given 2 framework names, this method returns a number which determines how compatible
	///       the names are. The higher the number the more compatible the frameworks are.
	///       </summary>
	private static int GetProfileCompatibility(FrameworkName frameworkName, FrameworkName targetFrameworkName)
	{
		int num = 0;
		if (NormalizeVersion(frameworkName.Version) == NormalizeVersion(targetFrameworkName.Version))
		{
			num++;
		}
		if (targetFrameworkName.Profile.Equals(frameworkName.Profile, StringComparison.OrdinalIgnoreCase))
		{
			num++;
		}
		return num;
	}
}

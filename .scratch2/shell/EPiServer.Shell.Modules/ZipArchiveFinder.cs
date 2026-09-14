using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EPiServer.Web;
using Microsoft.AspNetCore.Hosting;

namespace EPiServer.Shell.Modules;

internal class ZipArchiveFinder
{
	private readonly IWebHostEnvironment _hostingEnvironment;

	public ZipArchiveFinder(IWebHostEnvironment hostingEnvironment)
	{
		_hostingEnvironment = hostingEnvironment;
	}

	/// <summary>
	///       Finds all directories directly under the provided <paramref name="rootPath" /> that contains a
	///       ZIP archive with the same name as the directory.
	///       </summary>
	/// <param name="rootPath">The root path under where to look for directories with archives.</param>
	/// <returns>A dictionary where the key is the name of the directory and the value is the relative path to the archive.</returns>
	public virtual IDictionary<string, string> Find(string rootPath)
	{
		if (rootPath == null)
		{
			return null;
		}
		rootPath = VirtualPathUtilityEx.AppendTrailingSlash(rootPath);
		rootPath = rootPath.TrimStart('/');
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(_hostingEnvironment.ContentRootPath, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? rootPath.Replace('/', '\\') : rootPath));
		if (!directoryInfo.Exists)
		{
			return null;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (DirectoryInfo item in directoryInfo.EnumerateDirectories().Where(HasZipArchive))
		{
			dictionary.Add(item.Name, string.Format(CultureInfo.InvariantCulture, "{0}/{0}.zip", item.Name));
		}
		return dictionary;
	}

	private static bool HasZipArchive(DirectoryInfo directory)
	{
		if (directory.Name.Equals("EPiServer.Commerce.UI", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return directory.EnumerateFiles(directory.Name + ".zip").Any();
	}
}

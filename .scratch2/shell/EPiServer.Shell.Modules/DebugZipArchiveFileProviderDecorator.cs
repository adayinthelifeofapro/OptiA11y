using System;
using EPiServer.Web.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Modules;

public class DebugZipArchiveFileProviderDecorator : IBasePathFileProvider, IFileProvider
{
	private readonly ILogger<DebugZipArchiveFileProviderDecorator> _logger;

	private readonly IBasePathFileProvider _provider;

	public string BasePath => _provider.BasePath;

	public DebugZipArchiveFileProviderDecorator(IBasePathFileProvider fileProvider, ILogger<DebugZipArchiveFileProviderDecorator> logger)
	{
		_provider = fileProvider;
		_logger = logger;
	}

	public IFileInfo GetFileInfo(string subpath)
	{
		_logger.GetFile(subpath);
		if (IsJavascriptResource(subpath))
		{
			string text = subpath + ".uncompressed.js";
			IFileInfo fileInfo = ((IFileProvider)_provider).GetFileInfo(text);
			if (fileInfo != null)
			{
				_logger.FallingBackToUncompressed(text);
				return fileInfo;
			}
			if (subpath.Contains(".min.js"))
			{
				string text2 = subpath.Replace(".min", string.Empty);
				IFileInfo fileInfo2 = ((IFileProvider)_provider).GetFileInfo(text2);
				if (fileInfo2 != null)
				{
					_logger.FallingBackToUnminified(text2);
					return fileInfo2;
				}
			}
		}
		return ((IFileProvider)_provider).GetFileInfo(subpath);
	}

	/// <summary>
	///       We only need to support javascript files and can also skip language resources
	///       </summary>
	private bool IsJavascriptResource(string subpath)
	{
		if (subpath.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
		{
			return !subpath.Contains("/nls/", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public IDirectoryContents GetDirectoryContents(string subpath)
	{
		return ((IFileProvider)_provider).GetDirectoryContents(subpath);
	}

	public IChangeToken Watch(string filter)
	{
		return ((IFileProvider)_provider).Watch(filter);
	}
}

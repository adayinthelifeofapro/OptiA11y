using System;
using EPiServer.Web.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Modules;

internal class ReleaseFileProviderDecorator : IBasePathFileProvider, IFileProvider
{
	internal IBasePathFileProvider Provider { get; }

	public string BasePath => Provider.BasePath;

	public ReleaseFileProviderDecorator(IBasePathFileProvider fileProvider)
	{
		Provider = fileProvider;
	}

	public IFileInfo GetFileInfo(string subpath)
	{
		if (IsJavascriptSourceMap(subpath))
		{
			return new FakeMapFileInfo();
		}
		return ((IFileProvider)Provider).GetFileInfo(subpath);
	}

	private bool IsJavascriptSourceMap(string subpath)
	{
		return subpath.EndsWith(".js.map", StringComparison.OrdinalIgnoreCase);
	}

	public IDirectoryContents GetDirectoryContents(string subpath)
	{
		return ((IFileProvider)Provider).GetDirectoryContents(subpath);
	}

	public IChangeToken Watch(string filter)
	{
		return ((IFileProvider)Provider).Watch(filter);
	}
}

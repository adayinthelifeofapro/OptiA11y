using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace EPiServer.Shell.Modules;

internal class FakeMapFileInfo : IFileInfo
{
	public bool Exists => false;

	public bool IsDirectory => false;

	public DateTimeOffset LastModified => DateTimeOffset.MinValue;

	public long Length => 0L;

	public string Name => string.Empty;

	public string PhysicalPath => string.Empty;

	public Stream CreateReadStream()
	{
		return Stream.Null;
	}
}
